using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Hubs;
using Ridely.Contracts.Events;
using Ridely.Contracts.Models;
using Ridely.Domain.Riders;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;
using StackExchange.Redis;

namespace Ridely.Infrastructure.Consumers;
internal sealed class RideRequestedConsumer : IConsumer<RideRequestedEvent>
{
    private readonly IPushNotificationService _deviceNotificationService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RideRequestedConsumer> _logger;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly IDatabase _database;

    public RideRequestedConsumer(IConnectionMultiplexer connectionMultiplexer,
        IPushNotificationService deviceNotificationService,
        ApplicationDbContext context, ILogger<RideRequestedConsumer> logger,
        IHubContext<RideHub> rideHubContext)
    {
        _deviceNotificationService = deviceNotificationService;
        _context = context;
        _logger = logger;
        _rideHubContext = rideHubContext;
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
        if (ride is null) return;

        string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

        string rideCancelledKey = RideKeys.RideCancelled(ride.Id.ToString());

        var lastDriver = availableDriverProfiles.Last();

        foreach (var driverProfile in availableDriverProfiles)
        {
            int responseTime = ApplicationConstant.DRIVER_RESPONSETIME_INSECONDS;

            var riderDisconnected = await _database.StringGetAsync(RiderKey.Disconnected(rider.Id));

            if (riderDisconnected.HasValue) break;

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

            var rideRequestSignalRData = new
            {
                Source = ride.SourceAddress,
                Destination = ride.DestinationAddress,
                ride.Waypoints,
                RideId = ride.Id,
                ride.MusicGenre,
                ride.HaveConversation,
                ride.EstimatedFare,
                ride.PaymentMethod,
                ResponseTime = responseTime,
                Rider = new
                {
                    rider.FirstName,
                    rider.LastName,
                    rider.ProfileImageUrl,
                    rider.PhoneNo
                }
            };

            await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(driverProfile.Id))
                .SendAsync(SignalRSubscription.ReceiveRideRequests, rideRequestSignalRData, CancellationToken.None);

            var rideRequestSignalRDriverData = new
            {
                Driver = new
                {
                    driverProfile.FirstName,
                    driverProfile.LastName,
                    driverProfile.ProfileImageUrl,
                    driverProfile.Ratings
                }
            };

            await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(rider.Id))
                .SendAsync(SignalRSubscription.ReceiveRideRequests, rideRequestSignalRDriverData, CancellationToken.None);

            //string driverWebSocketKey = WebSocketKeys.Driver.Key(driverProfile.Id.ToString());

            //var driverRideRequestMessage = WebSocketMessage<object>.Create(
            //    SocketEventConstants.RIDE_REQUEST,
            //    new
            //    {
            //        source = ride.SourceAddress,
            //        waypoints = ride.Waypoints,
            //        destination = ride.DestinationAddress,
            //        rideId = ride.Id,
            //        musicGenre = ride.MusicGenre,
            //        conversation = ride.HaveConversation,
            //        estimatedFare = ride.EstimatedFare,
            //        paymentMethod = ride.PaymentMethod,
            //        responseTime,
            //        rider = new
            //        {
            //            firstName = rider.FirstName,
            //            lastName = rider.LastName,
            //            profileImage = rider.ProfileImageUrl,
            //            phoneNo = rider.PhoneNo
            //        }
            //    });

            //await _webSocketManager.SendMessageAsync(driverWebSocketKey, driverRideRequestMessage);

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
            //var riderRideRequestMessage = WebSocketMessage<object>.Create(
            //    SocketEventConstants.RIDER_NOTIFIED_DRIVERPOTENTIAL,
            //    new
            //    {
            //        driver = new
            //        {
            //            name = driverProfile.FirstName + " " + driverProfile.LastName,
            //            imageUrl = driverProfile.ProfileImageUrl
            //        }
            //    });

            //string riderWebSocketKey = WebSocketKeys.Rider.Key(rider.Id.ToString());
            //await _webSocketManager.SendMessageAsync(riderWebSocketKey, riderRideRequestMessage);

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
                    var rideRequestNoMatchData = new
                    {
                        Update = ReceiveRideUpdate.NoMatch,
                        Data = JsonSerializer.Serialize(new
                        {
                            Message = "No match"
                        })
                    };

                    await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(rider.Id))
                        .SendAsync(SignalRSubscription.ReceiveRideUpdates, 
                        rideRequestNoMatchData, CancellationToken.None);

                    // communicate to rider
                    //var noMatchMessage = WebSocketMessage<object>.Create(
                    //    SocketEventConstants.RIDE_NOMATCH,
                    //    new
                    //    {
                    //        match = false
                    //    });

                    //await _webSocketManager.SendMessageAsync(riderWebSocketKey, noMatchMessage);

                    //var riderDomain = await _context.Set<Rider>().FirstOrDefaultAsync(rdr => rdr.Id == rider.Id);

                    //if (riderDomain is null)
                    //{
                    //    Console.WriteLine("rider is null");
                    //    return;
                    //}

                    //riderDomain.UpdateStatus(RiderStatus.Online, 0);

                    //_context.Set<Rider>().Update(riderDomain);

                    //await _context.SaveChangesAsync();
                }
            }
        }
    }
}
