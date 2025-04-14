using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ridely.Application.Abstractions.Location;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Hubs;
using Ridely.Application.Models.Shared;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Cache;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;
using DriverDomain = Ridely.Domain.Drivers.Driver;

namespace Ridely.Application.Features.Rides.AcceptRejectRide;
internal sealed class AcceptRejectRideCommandHandler : 
    ICommandHandler<AcceptRejectRideCommand, AcceptRejectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPushNotificationService _deviceNotificationService;
    private readonly ILocationService _locationService;
    private readonly IRideRepository _rideRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IHubContext<RideHub> _rideHubContext;

    public AcceptRejectRideCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService,
        IPushNotificationService deviceNotificationService, ILocationService locationService,
        IRideRepository rideRepository, IDriverRepository driverRepository, IRiderRepository riderRepository,
        IRideLogRepository rideLogRepository, IHubContext<RideHub> rideHubContext)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _deviceNotificationService = deviceNotificationService;
        _locationService = locationService;
        _rideRepository = rideRepository;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _rideLogRepository = rideLogRepository;
        _rideHubContext = rideHubContext;
    }

    public async Task<Result<AcceptRejectResponse>> Handle(AcceptRejectRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound",
            "Ride not found");

        if (ride.DriverId.HasValue) return Error.BadRequest("ride.matched", "Ride has been matched to driver");

        string driverRideRequestKey = RideKeys.RideRequestToDriver(request.DriverId.ToString());

        string? driverRideRequestValue = await _cacheService.GetAsync(driverRideRequestKey);

        if (string.IsNullOrWhiteSpace(driverRideRequestValue))
            return Error.BadRequest("riderequest.timeout",
                $"Driver can respond to ride request within {ApplicationConstant.DRIVER_RESPONSETIME_INSECONDS} seconds");

        if (ride.Status != RideStatus.Requested)
            return Error.BadRequest("ride.notrequested", "Ride not requested");

        if (request.AcceptRide)
        {
            // To prevent the request from being sent to other drivers
            string rideMatchKey = RideKeys.RideNotMatched(ride.Id);

            await _cacheService.RemoveAsync(rideMatchKey);
        }

        else return await HandleRideRejection(request.DriverId, driverRideRequestKey, cancellationToken);

        // todo: handle this situation...this needs to be handled
        // 1.return from here
        // other options???
        // can it be ignored...perhaps not relevant??
        var riderDisconnected = await _cacheService.GetAsync(RiderKey.Disconnected(ride.RiderId));

        var driver = await _driverRepository
            .GetDetailsAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        var rideCancelled = await IsRideCancelled(ride.Id);

        if(rideCancelled) return new AcceptRejectResponse(new LocationResponse(0, 0), true, true);

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        Location sourceLocation = ride.GetCoordinates(ride.SourceCordinates);

        var (latitude, longitude, _) = await _locationService.GetDriverCoordinatesAsync(driver.Id);

        await NotifyRiderOfMatch(latitude, longitude, driver, ride, rider!);

        await UpdateCache(driver, ride, driver.Id);

        try
        {
            await PersistData(ride, driver, rider!, cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Error.BadRequest("error", "An error occurred");
        }

        return new AcceptRejectResponse(new LocationResponse(sourceLocation.Latitude,
            sourceLocation.Longitude), true, false);
    }

    private async Task<bool> IsRideCancelled(long rideId)
    {
        string matchProcessCancellationKey = RideKeys.RideCancelled(rideId.ToString());

        var matchCancellationValue = await _cacheService.GetAsync(matchProcessCancellationKey);

        return !string.IsNullOrWhiteSpace(matchCancellationValue);
    }

    private async Task<AcceptRejectResponse> HandleRideRejection(long driverId, string driverRideRequestKey,
        CancellationToken cancellationToken)
    {
        var driver_ = await _driverRepository
               .GetAsync(driverId);

        if (driver_ is null)
            return new AcceptRejectResponse(new LocationResponse(0, 0), false, false);

        await _cacheService.RemoveAsync(driverRideRequestKey);

        driver_.UpdateDeclinedRides();

        _driverRepository.Update(driver_);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AcceptRejectResponse(new LocationResponse(0, 0), false, false);
    }

    private async Task NotifyRiderOfMatch(double latitude, double longitude,
        DriverDomain driver, Domain.Rides.Ride ride, Domain.Riders.Rider rider)
    {
        await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(rider.Id))
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                Update = ReceiveRideUpdate.Accepted,
                Data = JsonSerializer.Serialize(new
                {
                    DriverLocation = new
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                    },
                    Driver = new
                    {
                        Name = driver.FirstName + " " + driver.LastName,
                        driver.PhoneNo,
                        driver.ProfileImageUrl,
                        Ratings = driver.AvgRatings
                    },
                    Cab = new
                    {
                        driver.Cab.LicensePlateNo,
                        driver.Cab.Color,
                        driver.Cab.Name,
                        driver.Cab.Model,
                        driver.Cab.Manufacturer
                    },
                    Ride = new
                    {
                        RideId = ride.Id
                    }
                })
            });

        //var rideMatchedMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDE_MATCHED,
        //    new
        //    {
        //        driverLocation = new
        //        {
        //            latitude,
        //            longitude
        //        },
        //        driver = new
        //        {
        //            name = driver.FirstName + " " + driver.LastName,
        //            phoneNo = driver.PhoneNo,
        //            imageUrl = driver.ProfileImageUrl
        //        },
        //        cab = new
        //        {
        //            licensePlateNo = driver.Cab.LicensePlateNo,
        //            color = driver.Cab.Color,
        //            name = driver.Cab.Name,
        //            model = driver.Cab.Model,
        //            manufacturer = driver.Cab.Manufacturer
        //        },
        //        ride = new
        //        {
        //            rideId = ride.Id
        //        }
        //    });

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        //await _webSocketManager.SendMessageAsync(riderWebSocketKey, rideMatchedMessage);

        if (!string.IsNullOrWhiteSpace(rider.DeviceTokenId))
        {
            Dictionary<string, string> data = new()
            {
                {"rideId", ride.Id.ToString() }
            };

            await _deviceNotificationService.PushAsync(
                rider.DeviceTokenId,
                "Ride matched",
                "You have been matched to a driver",
                data, PushNotificationType.RideMatched);
        }
    }

    private async Task UpdateCache(DriverDomain driver, Domain.Rides.Ride ride, long driverId)
    {
        // Set this so future driver location updates can be forwarded to rider
        string driverMatchedKey = RideKeys.Matched(driver.Id.ToString());

        await _cacheService.SetAsync(driverMatchedKey, ride.RiderId.ToString(), TimeSpan.FromHours(10));

        // todo: add the respective device token ids...
        RideCacheModel rideCacheModel = new(ride.Id, ride.RiderId, driverId, "", "");

        string rideObjectKey = RideKeys.Ride(ride.Id.ToString());

        await _cacheService.SetAsync(rideObjectKey, JsonSerializer.Serialize(rideCacheModel), TimeSpan.FromHours(12));

        await _locationService.DeleteLocationRecordAsync(driverId);
    }

    private async Task PersistData(Domain.Rides.Ride ride, DriverDomain driver, Domain.Riders.Rider rider,
        CancellationToken cancellationToken)
    {
        driver.UpdateStatus(DriverStatus.Matched, ride.Id);

        rider!.UpdateStatus(RiderStatus.Matched, ride.Id);

        ride.UpdateStatus(RideStatus.Matched, driverId: driver.Id);

        _rideRepository.Update(ride);

        _driverRepository.Update(driver);

        _riderRepository.Update(rider);

        RideLog rideLog = new(ride.Id, RideLogEvent.Matched);

        await _rideLogRepository.AddAsync(rideLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
