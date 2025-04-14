using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Abstractions.Settings;
using Ridely.Application.Features.Rides.CancelRideRequest;
using Ridely.Application.Hubs;
using Ridely.Contracts.Events;
using Ridely.Contracts.Models;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models.Cache;
using Ridely.Domain.Models.Rides;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Infrastructure.Location;
using Ridely.Infrastructure.Route;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;
using StackExchange.Redis;

namespace Ridely.Infrastructure.Rides;
internal sealed class RideService : IRideService
{
    private readonly IDatabase _database;
    private readonly ApplicationDbContext _context;
    private readonly RouteService _routeService;
    private readonly IPushNotificationService _pushNotificationService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RideService> _logger;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly ApplicationSettings _applicationSettings;

    public RideService(IConnectionMultiplexer connectionMultiplexer,
        ApplicationDbContext context, RouteService routeService, 
        IPushNotificationService pushNotificationService, 
        IPublishEndpoint publishEndpoint,
        ILogger<RideService> logger, IHubContext<RideHub> rideHubContext,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _database = connectionMultiplexer.GetDatabase();
        _context = context;
        _routeService = routeService;
        _pushNotificationService = pushNotificationService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _rideHubContext = rideHubContext;
        _applicationSettings = applicationSettings.Value;
    }

    public async Task<bool> SendRequestToDriversAsync(Domain.Models.Location ridersLocation, RideObject ride,
       Rider rider, List<long> excludeDrivers, RideCategory rideCategory, bool increaseSearchRadius = false)
    {
        // First request, we use 3000M
        // Subsequent request if no driver, increase to 5000M
        double radius;

        if (increaseSearchRadius) radius = 5000;
        else radius = 3000;

        // Ascending -> closest drivers to farthest
        var result = await _database.GeoSearchAsync(ApplicationConstant.REDIS_LOCATIONKEY,
            ridersLocation.Longitude, ridersLocation.Latitude, new GeoSearchCircle(radius, GeoUnit.Meters),
            order: Order.Ascending, options: GeoRadiusOptions.WithDistance);

        // x.Member.ToString sample --> DRIVER.LOCATION-2
        var drivers = result.Select(x => new DriversAvailable
        (
            x.Member.ToString(),
            x.Distance,
            x.Member.ToString().Split('-').Length > 1 ? x.Member.ToString().Split('-')[1] : "0"
        ));

        DriverService driverService = DriverService.Unknown;

        CabType cabType = CabType.Unknown;

        if (rideCategory == RideCategory.CabEconomy)
        {
            cabType = CabType.Economy;
            driverService = DriverService.Car;
        }
        else if (rideCategory == RideCategory.CabPremium)
        {
            cabType = CabType.Premium;
            driverService = DriverService.Car;
        }
        else
        {
            driverService = DriverService.Delivery;
        }

        excludeDrivers.AddRange(await GetCancelledDrivers(rider.Id.ToString()));

        // todo: very possible that majority here would not be available...
        // review: the first 20 would always get...when there's a match, we remove the driver from store and just forward...
        long[] availableDriversIds = drivers
            .Select(driver => long.Parse(driver.DriverId))
            .Where(identifier => !excludeDrivers.Contains(identifier))
            .Take(20)
            .ToArray();

        if (availableDriversIds.Length < 1) return false;

        int deficitWalletAmount = _applicationSettings.MaximumFundDeficitFromDriver;

        // todo: Include drivers that are completing trips nearby...to be done later
        // might use dapper...
        var availableDriverProfiles = await _context.Set<Driver>()
            .Join(_context.Set<DriverWallet>(), dr => dr.Id, wal => wal.DriverId, (dr, wal) => new {dr, wal})
            .Join(_context.Set<Cab>(), agg => agg.dr.CabId, cb => cb.Id, (agg, cb) => new {agg.dr, agg.wal, cb})
            .Where(agg => availableDriversIds.Contains(agg.dr.Id))
            .Where(agg => cabType != CabType.Unknown && agg.cb.CabType == cabType)
            .Where(agg => driverService != DriverService.Unknown && agg.dr.DriverService == driverService)
            .Where(agg => agg.dr.Status == DriverStatus.Online)
            .Where(agg => agg.dr.CurrentRideId == null)
            .Where(agg => agg.dr.IdentityValidated)
            .Where(agg => !agg.dr.IsBarred && !agg.dr.IsDeactivated && !agg.dr.IsDeleted)
            .Where(agg => agg.wal.AvailableBalance > deficitWalletAmount)
            .OrderByDescending(agg => agg.dr.CompletedTrips)
            .ThenByDescending(agg => agg.dr.AvgRatings)
            .Select(agg => new DriverProfile(
                agg.dr.Id,
                agg.dr.FirstName,
                agg.dr.LastName,
                agg.dr.DeviceTokenId ?? "",
                agg.dr.ProfileImageUrl,
                agg.dr.AvgRatings
                ))
            .AsNoTracking()
            .ToListAsync();

        if (availableDriverProfiles.Count == 0) return false;

        RiderProfile riderProfile = new(
            rider.Id,
            rider.FirstName,
            rider.LastName,
            rider.ProfileImageUrl,
            rider.PhoneNo);

        await _publishEndpoint.Publish(new RideRequestedEvent
        {
            AvailableDriverProfile = availableDriverProfiles,
            Ride = ride,
            Rider = riderProfile
        });

        // Send requests to drivers
        //BackgroundJob.Enqueue(() => SendRideRequestToAvailableDrivers(availableDriverProfiles, 
        //    ride, riderProfile));

        //await SendRideRequestToAvailableDrivers(availableDrivers, ride, riderProfile);

        return true;
    }

    public async Task SendChatMessageAsync(UserType sender, string message, 
        string identifier, long rideId)
    {
        long riderId = 0;
        long driverId = 0;

        string rideObjectKey = RideKeys.Ride(rideId.ToString());

        string? rideObject = await _database.StringGetAsync(rideObjectKey);

        RideCacheModel? rideData;

        if (!string.IsNullOrWhiteSpace(rideObject))
        {
            rideData = JsonSerializer.Deserialize<RideCacheModel>(rideObject);

            if (rideData is not null)
            {
                riderId = rideData.RiderId;
                driverId = rideData.DriverId;
            }
        }
        else
        {
            // Trip has been completed...
            return;
        }

        if (riderId == default || driverId == default)
        {
            var ride = await _context.Set<Ride>()
                .FirstOrDefaultAsync(x => x.Id == rideId);

            if (ride is not null)
            {
                driverId = ride.DriverId!.Value;
                riderId = ride.RiderId;
            }
        }

        if (riderId == default || driverId == default) return;

        Chat chat = new(rideId, UserType.Driver, UserType.Rider, message);

        if (sender == UserType.Rider)
            chat.SetParties(UserType.Rider, UserType.Driver);

        await _context.Set<Chat>().AddAsync(chat);

        await _context.SaveChangesAsync();

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(riderId.ToString());

        //string driverWebSocketKey = WebSocketKeys.Driver.Key(driverId.ToString());

        var chats = await _context.Set<Chat>()
            .Where(x => x.RideId == rideId)
            .Select(x => new ChatModel
            {
                CreatedAt = x.CreatedAtUtc,
                Message = x.Message,
                Recipient = x.Recipient.ToString(),
                Sender = x.Sender.ToString()
            })
            .OrderByDescending(x => x.CreatedAt)
            //.Take(20)
            .ToListAsync();

        //var chatResponseMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDE_CHAT_RESPONSE,
        //    new
        //    {
        //        messages = chats
        //    });

        string riderIdentifier = RiderKey.CustomNameIdentifier(riderId);
        string driverIdentifier = DriverKey.CustomNameIdentifier(driverId);

        await _rideHubContext.Clients.Users(riderIdentifier, driverIdentifier)
            .SendAsync(SignalRSubscription.ReceiveChatMessage, new
            {
                Chats = chats
            });

        Dictionary<string, string> notificationData = new();

        if (sender == UserType.Rider && rideData is not null && !string.IsNullOrWhiteSpace(rideData.DriverDeviceTokenId))
            await _pushNotificationService.PushAsync(rideData.DriverDeviceTokenId, 
                "New message from rider", "You have a message from your rider", notificationData, PushNotificationType.Chat);

        else if (sender == UserType.Driver && rideData is not null && !string.IsNullOrWhiteSpace(rideData.RiderDeviceTokenId))
            await _pushNotificationService.PushAsync(rideData.RiderDeviceTokenId,
                "New message from driver", "You have a message from your driver", notificationData, PushNotificationType.Chat);
    }

    public async Task<Result<EstimatedFareResponse>> ComputeEstimatedFare(
        Domain.Models.Location source, Domain.Models.Location destination,
        Domain.Models.Location? waypoint)
    {
        var request = new RouteRequest
        {
            Origin = new RouteRequest.RouteRequestLocation
            {
                Location = new RouteRequest.RouteRequestLocation.RequestLocation
                {
                    LatLng = new RouteRequest.RouteRequestLocation.RequestLocation.Coordinates
                    {
                        Latitude = source.Latitude,
                        Longitude = source.Longitude
                    }
                }
            },
            Destination = new RouteRequest.RouteRequestLocation
            {
                Location = new RouteRequest.RouteRequestLocation.RequestLocation
                {
                    LatLng = new RouteRequest.RouteRequestLocation.RequestLocation.Coordinates
                    {
                        Latitude = destination.Latitude,
                        Longitude = destination.Longitude
                    }
                }
            }
        };

        if(waypoint is not null)
        {
            request.Intermediates = new()
            {
                new RouteRequest.RouteRequestLocation
                {
                    Location = new RouteRequest.RouteRequestLocation.RequestLocation
                    {
                        LatLng = new RouteRequest.RouteRequestLocation.RequestLocation.Coordinates
                        {
                            Latitude = waypoint.Latitude,
                            Longitude = waypoint.Longitude
                        }
                    }
                }
            };
        }

        var response = await _routeService.GetDistanceAndDurationAsync(request);

        if (response.IsFailure) return response.Error;

        // todo: cache settings...
        var settings = await _context.Set<Settings>()
            .ToListAsync();

        if (settings.Count < 1)
            throw new ApplicationException("Settings not found");

        var setting = settings.First();

        decimal ratePerMinute = setting.RatePerMinute;
        decimal baseFare = setting.BaseFare;
        decimal ratePerKilometer = setting.RatePerKilometer;
        decimal deliveryRatePerKm = setting.DeliveryRatePerKilometer;

        var route = response.Value;

        int duration = int.Parse(route.Duration.Replace("s", ""));

        decimal distancePrice = baseFare + ((decimal)(route.DistanceMeters / 1000) * ratePerKilometer);

        decimal timePrice = (decimal)(duration / 60) * ratePerMinute;

        long estimatedFare = (long)(distancePrice + timePrice);

        long deliveryFare = (long)(deliveryRatePerKm * (route.DistanceMeters / 1000));

        return new EstimatedFareResponse(estimatedFare, deliveryFare, duration, route.DistanceMeters);
    }

    //[Queue("high")]
    //public async Task SendRideRequestToAvailableDrivers(
    //    List<DriverProfile> availableDriverProfiles,
    //    RideObject ride, RiderProfile rider)
    //{
    //    string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

    //    string rideCancelledKey = RideKeys.RideCancelled(ride.Id.ToString());

    //    var lastDriver = availableDriverProfiles.Last();

    //    foreach (var driverProfile in availableDriverProfiles)
    //    {
    //        int responseTime = ApplicationConstant.DRIVER_RESPONSETIME_INSECONDS;

    //        var rideNotMatchedvalue = await _database.StringGetAsync(rideNotMatchedKey);
    //        if (rideNotMatchedvalue.IsNull) break;

    //        var matchCancelledValue = await _database.StringGetAsync(rideCancelledKey);
    //        if (matchCancelledValue.HasValue)
    //        {
    //            await _database.KeyDeleteAsync(rideCancelledKey);
    //            break;
    //        }

    //        string driverRideRequestKey = RideKeys.RideRequestToDriver(driverProfile.Id.ToString());
    //        var driverRideRequestvalue = await _database.StringGetAsync(driverRideRequestKey);

    //        // If value exist, request has been sent to driver
    //        if (driverRideRequestvalue.HasValue) continue;

    //        await _database.StringSetAsync(driverRideRequestKey, 1,
    //            expiry: TimeSpan.FromSeconds(responseTime + 1));

    //        string driverWebSocketKey = WebSocketKeys.Driver.Key(driverProfile.Id.ToString());
    //        var driverRideRequestMessage = WebSocketMessage<object>.Create(
    //            SocketEventConstants.RIDE_REQUEST,
    //            new
    //            {
    //                source = ride.SourceAddress,
    //                destination = ride.DestinationAddress,
    //                rideId = ride.Id,
    //                musicGenre = ride.MusicGenre,
    //                conversation = ride.HaveConversation,
    //                estimatedFare = ride.EstimatedFare,
    //                paymentMethod = ride.PaymentMethod,
    //                responseTime,
    //                rider = new
    //                {
    //                    firstName = rider.FirstName,
    //                    lastName = rider.LastName,
    //                    profileImage = rider.ProfileImageUrl,
    //                    phoneNo = rider.PhoneNo
    //                }
    //            });

    //        // Send request message to driver
    //        await _webSocketManager.SendMessageAsync(driverWebSocketKey, driverRideRequestMessage);

    //        if (!string.IsNullOrWhiteSpace(driverProfile.DeviceTokenId))
    //        {
    //            Dictionary<string, string> data = new()
    //            {
    //                {"rideId", ride.Id.ToString()},
    //                {"responseTime", responseTime.ToString()}
    //            };

    //            await _deviceNotificationService.PushAsync(
    //                driverProfile.DeviceTokenId,
    //                "Ride request",
    //                "New ride request. Click to view details.",
    //                data, PushNotificationType.RideRequest);
    //        }

    //        // Communicate to rider
    //        var riderRideRequestMessage = WebSocketMessage<object>.Create(
    //            SocketEventConstants.RIDER_NOTIFIED_DRIVERPOTENTIAL,
    //            new
    //            {
    //                driver = new
    //                {
    //                    name = driverProfile.FirstName + " " + driverProfile.LastName,
    //                    imageUrl = driverProfile.ProfileImageUrl
    //                }
    //            });

    //        string riderWebSocketKey = WebSocketKeys.Rider.Key(rider.Id.ToString());
    //        await _webSocketManager.SendMessageAsync(riderWebSocketKey, riderRideRequestMessage);

    //        // Add .5 second 
    //        await Task.Delay((responseTime * 1000) + 500);

    //        // if driver is last driver
    //        // after the delay, we see if ride was accepted
    //        if (driverProfile.Id == lastDriver.Id)
    //        {
    //            string ridePendingKey = RideKeys.RideNotMatched(ride.Id);

    //            var ridePendingValue = await _database.StringGetAsync(ridePendingKey);

    //            // if there's a value, then ride match is still pending and request sent to all available drivers
    //            if (ridePendingValue.HasValue)
    //            {
    //                // communicate to rider
    //                var noMatchMessage = WebSocketMessage<object>.Create(
    //                    SocketEventConstants.RIDE_NOMATCH,
    //                    new
    //                    {
    //                        match = false
    //                    });

    //                await _webSocketManager.SendMessageAsync(riderWebSocketKey, noMatchMessage);

    //                // todo: possible refactor??
    //                var riderDomain = await _context.Set<Rider>().FirstOrDefaultAsync(rider => rider.Id == rider.Id);

    //                if (riderDomain is null) return;

    //                riderDomain.UpdateStatus(RiderStatus.Online, null);

    //                _context.Set<Rider>().Update(riderDomain);

    //                await _context.SaveChangesAsync();
    //            }
    //        }
    //    }
    //}
    private async Task<List<long>> GetCancelledDrivers(string riderId)
    {
        string key = RideKeys.DriversCancelled(riderId);

        var cancelledDrivers = await _database.StringGetAsync(key);

        if (cancelledDrivers.IsNullOrEmpty) return [];

        var drivers = JsonSerializer.Deserialize<List<CancelledDrivers>>(cancelledDrivers!) ?? [];

        List<long> driverIds = [];

        driverIds.AddRange(drivers
            .Where(driver => driver.Expiry >= DateTime.UtcNow)
            .Select(driver => driver.DriverId)
            .ToList());

        return driverIds;
    }
}

