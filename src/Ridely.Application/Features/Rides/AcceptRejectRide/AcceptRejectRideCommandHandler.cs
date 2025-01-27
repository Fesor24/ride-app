using System.Text.Json;
using Hangfire;
using MediatR;
using Soloride.Application.Abstractions.Location;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Notifications;
using Soloride.Application.Abstractions.Payment;
using Soloride.Application.Abstractions.Websocket;
using Soloride.Application.Features.Rides.CancelRideRequest;
using Soloride.Application.Models.Shared;
using Soloride.Application.Models.WebSocket;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Cache;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Services;
using Soloride.Shared.Constants;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;
using DriverDomain = Soloride.Domain.Drivers.Driver;

namespace Soloride.Application.Features.Rides.AcceptRejectRide;
internal sealed class AcceptRejectRideCommandHandler : 
    ICommandHandler<AcceptRejectRideCommand, AcceptRejectResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IWebSocketManager _webSocketManager;
    private readonly IDeviceNotificationService _deviceNotificationService;
    private readonly ILocationService _locationService;
    private readonly IRideRepository _rideRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentService _paymentService;
    private readonly ISender _sender;

    public AcceptRejectRideCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService,
        IWebSocketManager webSocketManager,
        IDeviceNotificationService deviceNotificationService, ILocationService locationService,
        IRideRepository rideRepository, IDriverRepository driverRepository, IRiderRepository riderRepository,
        IRideLogRepository rideLogRepository,  IPaymentService paymentService,
        ISender sender)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _webSocketManager = webSocketManager;
        _deviceNotificationService = deviceNotificationService;
        _locationService = locationService;
        _rideRepository = rideRepository;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _rideLogRepository = rideLogRepository;
        _paymentService = paymentService;
        _sender = sender;
    }

    public async Task<Result<AcceptRejectResponse>> Handle(AcceptRejectRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound",
            "Ride not found");

        if (ride.DriverId.HasValue) return Error.BadRequest("ride.matched", "Ride has been matched to driver");

        // todo: set concurrency for ride...still uncertain though...think abt it if it is needed...
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

        else await HandleRideRejection(request.DriverId, driverRideRequestKey, cancellationToken);

        var driver = await _driverRepository
            .GetDetailsAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        var rideCancelled = await CheckRideCancellationStatus(ride.Id, driver.Id);

        if(rideCancelled) return new AcceptRejectResponse(new LocationResponse(0, 0), true, true);

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        Location sourceLocation = ride.GetCoordinates(ride.SourceCordinates);

        string driversLocationKey = RideKeys.DriverLocation(driver.Id.ToString());

        var (latitude, longitude) = await _locationService.GetDriverCoordinatesAsync(driversLocationKey);

        await NotifyRiderOfMatch(latitude, longitude, driver, ride, rider!);

        await UpdateCache(driver, ride, driver.Id);

        await PersistData(ride, driver, rider!, cancellationToken);

        return new AcceptRejectResponse(new LocationResponse(sourceLocation.Latitude,
            sourceLocation.Longitude), true, false);
    }

    private async Task<bool> CheckRideCancellationStatus(long rideId, long driverId)
    {
        string matchProcessCancellationKey = RideKeys.RideCancelled(rideId.ToString());

        var matchCancellationValue = await _cacheService.GetAsync(matchProcessCancellationKey);

        if (!string.IsNullOrWhiteSpace(matchCancellationValue))
        {
            string driverWebSocketKey = WebSocketKeys.Driver.Key(driverId.ToString());

            var cancellationMessage = new WebSocketResponse<object>
            {
                Event = SocketEventConstants.RIDE_CANCELLATION,
                Payload = new { message = "Ride request cancelled" }
            };

            string message = Serialize.Object(cancellationMessage);

            await _webSocketManager.SendMessageAsync(driverWebSocketKey, message);

            return true;
        }

        return false;
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
        var rideMatchedMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.RIDE_MATCHED,
            Payload = new
            {
                driverLocation = new
                {
                    latitude,
                    longitude
                },
                driver = new
                {
                    name = driver.FirstName + " " + driver.LastName,
                    phoneNo = driver.PhoneNo,
                    imageUrl = driver.ProfileImageUrl
                },
                cab = new
                {
                    licensePlateNo = driver.Cab.LicensePlateNo,
                    color = driver.Cab.Color,
                    name = driver.Cab.Name,
                    model = driver.Cab.Model,
                    manufacturer = driver.Cab.Manufacturer
                },
                ride = new
                {
                    rideId = ride.Id
                }
            }
        };

        string riderMatchMessage = Serialize.Object(rideMatchedMessage);

        string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        // Notify rider with details
        await _webSocketManager.SendMessageAsync(riderWebSocketKey, riderMatchMessage);

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

        // cache for chat??
        RideCacheModel rideCacheModel = new(ride.Id, ride.RiderId, driverId);

        string rideObjectKey = RideKeys.Ride(ride.Id.ToString());

        await _cacheService.SetAsync(rideObjectKey, JsonSerializer.Serialize(rideCacheModel), TimeSpan.FromHours(12));
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

        RideLog rideLog = new(ride.Id, RideStatus.Matched);

        await _rideLogRepository.AddAsync(rideLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
