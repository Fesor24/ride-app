using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Contracts.Events;
using Ridely.Contracts.Models;
using Ridely.Domain.Riders;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;
using StackExchange.Redis;

namespace Ridely.Infrastructure.Consumers;
internal sealed class RideRequestedConsumer : IConsumer<RideRequestedEvent>
{
    private readonly IWebSocketManager _webSocketManager;
    private readonly IDeviceNotificationService _deviceNotificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RideRequestedConsumer> _logger;
    private readonly IDatabase _database;

    public RideRequestedConsumer(IWebSocketManager webSocketManager,
        IConnectionMultiplexer connectionMultiplexer,
        IDeviceNotificationService deviceNotificationService,
        ApplicationDbContext context, ILogger<RideRequestedConsumer> logger)
    {
        _webSocketManager = webSocketManager;
        _deviceNotificationService = deviceNotificationService;
        _context = context;
        _logger = logger;
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task Consume(ConsumeContext<RideRequestedEvent> context)
    {
        await SendRequestToAvailableDriversAsync(context.Message.AvailableDriverProfile,
            context.Message.Ride, context.Message.Rider);
    }

    private async Task SendRequestToAvailableDriversAsync(List<DriverProfile> availableDriverProfiles,
        RideObject ride, RiderProfile rider)
    {
        _logger.LogInformation("Processing ride requests...");

        if(ride is null)
        {
            _logger.LogInformation("Ride object is null");
            return;
        }

        string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

        string rideCancelledKey = RideKeys.RideCancelled(ride.Id.ToString());

        var lastDriver = availableDriverProfiles.Last();

        foreach (var driverProfile in availableDriverProfiles)
        {
            int responseTime = ApplicationConstant.DRIVER_RESPONSETIME_INSECONDS;

            var rideNotMatchedvalue = await _database.StringGetAsync(rideNotMatchedKey);
            if (rideNotMatchedvalue.IsNull) break;

            var matchCancelledValue = await _database.StringGetAsync(rideCancelledKey);
            if (matchCancelledValue.HasValue)
            {
                await _database.KeyDeleteAsync(rideCancelledKey);
                break;
            }

            string driverRideRequestKey = RideKeys.RideRequestToDriver(driverProfile.Id.ToString());
            var driverRideRequestvalue = await _database.StringGetAsync(driverRideRequestKey);

            // If value exist, request has been sent to driver
            if (driverRideRequestvalue.HasValue) continue;

            await _database.StringSetAsync(driverRideRequestKey, 1,
                expiry: TimeSpan.FromSeconds(responseTime + 1));

            string driverWebSocketKey = WebSocketKeys.Driver.Key(driverProfile.Id.ToString());
            var driverRideRequestMessage = new WebSocketResponse<object>
            {
                Event = SocketEventConstants.RIDE_REQUEST,
                Payload = new
                {
                    source = ride.SourceAddress,
                    destination = ride.DestinationAddress,
                    rideId = ride.Id,
                    musicGenre = ride.MusicGenre,
                    conversation = ride.HaveConversation,
                    estimatedFare = ride.EstimatedFare,
                    paymentMethod = ride.PaymentMethod,
                    responseTime,
                    rider = new
                    {
                        firstName = rider.FirstName,
                        lastName = rider.LastName,
                        profileImage = rider.ProfileImageUrl,
                        phoneNo = rider.PhoneNo
                    }
                }
            };

            string requestMessage = Serialize.Object(driverRideRequestMessage);
            // Send request message to driver
            await _webSocketManager.SendMessageAsync(driverWebSocketKey, requestMessage);

            if (!string.IsNullOrWhiteSpace(driverProfile.DeviceTokenId))
            {
                Dictionary<string, string> data = new()
                {
                    {"rideId", ride.Id.ToString()},
                    {"responseTime", responseTime.ToString()}
                };

                await _deviceNotificationService.PushAsync(
                    driverProfile.DeviceTokenId,
                    "Ride request",
                    "New ride request. Click to view details.",
                    data, PushNotificationType.RideRequest);
            }

            // Communicate to rider
            var riderRideRequestMessage = new WebSocketResponse<object>
            {
                Event = SocketEventConstants.RIDER_NOTIFIED_DRIVERPOTENTIAL,
                Payload = new
                {
                    driver = new
                    {
                        name = driverProfile.FirstName + " " + driverProfile.LastName,
                        imageUrl = driverProfile.ProfileImageUrl
                    }
                }
            };

            string riderRequestMessage = Serialize.Object(riderRideRequestMessage);
            string riderWebSocketKey = WebSocketKeys.Rider.Key(rider.Id.ToString());
            await _webSocketManager.SendMessageAsync(riderWebSocketKey, riderRequestMessage);

            // Add .5 second 
            await Task.Delay((responseTime * 1000) + 500);

            // if driver is last driver
            // after the delay, we see if ride was accepted
            if (driverProfile.Id == lastDriver.Id)
            {
                string ridePendingKey = RideKeys.RideNotMatched(ride.Id);

                var ridePendingValue = await _database.StringGetAsync(ridePendingKey);

                // if there's a value, then ride match is still pending and request sent to all available drivers
                if (ridePendingValue.HasValue)
                {
                    // communicate to rider
                    var noMatchMessage = new WebSocketResponse<object>
                    {
                        Event = SocketEventConstants.RIDE_NOMATCH,
                        Payload = new
                        {
                            match = false
                        }
                    };

                    string noMatchRequestMessage = Serialize.Object(noMatchMessage);
                    await _webSocketManager.SendMessageAsync(riderWebSocketKey, noMatchRequestMessage);

                    // todo: possible refactor??
                    var riderDomain = await _context.Set<Rider>().FirstOrDefaultAsync(rider => rider.Id == rider.Id);

                    if (riderDomain is null) return;

                    riderDomain.UpdateStatus(RiderStatus.Online, null);

                    _context.Set<Rider>().Update(riderDomain);

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
