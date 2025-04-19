using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.VoiceCall;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.VoiceCall.NotifyRecipient
{
    internal sealed class NotifyRecipientCommandHandler :
        ICommandHandler<NotifyRecipientCommand>
    {
        private readonly IRideRepository _rideRepository;
        private readonly IRiderRepository _riderRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IPushNotificationService _deviceNotificationService;
        private readonly IVoiceService _voiceService;
        private readonly IHubContext<RideHub> _rideHubContext;

        public NotifyRecipientCommandHandler(IRideRepository rideRepository,
            IRiderRepository riderRepository, IDriverRepository driverRepository,
            IPushNotificationService deviceNotificationService,
            IVoiceService voiceService, IHubContext<RideHub> rideHubContext)
        {
            _rideRepository = rideRepository;
            _riderRepository = riderRepository;
            _driverRepository = driverRepository;
            _deviceNotificationService = deviceNotificationService;
            _voiceService = voiceService;
            _rideHubContext = rideHubContext;
        }

        public async Task<Result<bool>> Handle(NotifyRecipientCommand request, CancellationToken cancellationToken)
        {
            var ride = await _rideRepository
                .GetAsync(request.RideId);

            if (ride is null)
                return Error.NotFound("ride.notfound", "Ride not found");

            // 12/23/2024 12:27:10 PM
            string callInitiatedTime = DateTime.UtcNow.ToString();

            if (request.RiderId.HasValue)
            {
                if (!ride.DriverId.HasValue)
                    return Error.BadRequest("ride.notmatched", "Ride not matched");

                var rider = await _riderRepository
                    .GetAsync(ride.RiderId);

                if (rider is null) return true;

                //string driverWebSocketKey = WebSocketKeys.Driver.Key(ride.DriverId.Value.ToString());

                //token for the driver...
                var (token, channel) = await _voiceService
                    .GenerateAgoraAccessTokenAsync(ride.Id.ToString(), true);

                var data = new Dictionary<string, string>
                {
                    {"expiryInSeconds", ApplicationConstant.CALL_WAIT_TIME.ToString() },
                    {"createdAt", callInitiatedTime },
                };

                //await _webSocketManager.SendMessageAsync(driverWebSocketKey, callRouteMessage);

                // notify driver...
                await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(ride.DriverId!.Value))
                    .SendAsync(SignalRSubscription.ReceiveCallNotification, new
                    {
                        CreatedAt = callInitiatedTime,
                        ExpiryInSeconds = ApplicationConstant.CALL_WAIT_TIME,
                        RideId = ride.Id,
                        CallerInfo = new
                        {
                            Caller = "rider",
                            Name = rider.FirstName + " " + rider.LastName,
                            ProfileImage = rider.ProfileImageUrl
                        },
                        CallInfo = new
                        {
                            Recipient = "driver",
                            Token = token,
                            Channel = channel
                        }
                    }, cancellationToken);

                var driver = await _driverRepository
                    .GetAsync(ride.DriverId.Value);

                if (driver is null || string.IsNullOrWhiteSpace(driver.DeviceTokenId))
                    return true;

                return await _deviceNotificationService
                    .PushAsync(driver.DeviceTokenId, "Call incoming", "Incoming call", 
                    data, PushNotificationType.Call);
            }

            if (request.DriverId.HasValue)
            {
                if (!ride.DriverId.HasValue)
                    return Error.BadRequest("ride.notmatched", "Ride not matched");

                var driver = await _driverRepository
                    .GetAsync(ride.DriverId!.Value);

                if (driver is null) return true;

                //string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

                //token for the rider...
                var (token, channel) = await _voiceService
                    .GenerateAgoraAccessTokenAsync(ride.Id.ToString(), false);

                var data = new Dictionary<string, string>
                {
                    {"expiryInSeconds", ApplicationConstant.CALL_WAIT_TIME.ToString() },
                    {"createdAt", callInitiatedTime },
                };

                //await _webSocketManager.SendMessageAsync(riderWebSocketKey, callRouteMessage);

                // notify rider...
                await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(ride.RiderId))
                   .SendAsync(SignalRSubscription.ReceiveCallNotification, new
                   {
                       CreatedAt = callInitiatedTime,
                       ExpiryInSeconds = ApplicationConstant.CALL_WAIT_TIME,
                       RideId = ride.Id,
                       CallerInfo = new
                       {
                           Caller = "driver",
                           Name = driver.FirstName + " " + driver.LastName,
                           ProfileImage = driver.ProfileImageUrl
                       },
                       CallInfo = new
                       {
                           Recipient = "rider",
                           Token = token,
                           Channel = channel
                       }
                   }, cancellationToken);

                var rider = await _riderRepository
                    .GetAsync(ride.RiderId);

                if (rider is null || string.IsNullOrWhiteSpace(rider.DeviceTokenId))
                    return true;

                return await _deviceNotificationService
                    .PushAsync(rider.DeviceTokenId, 
                    "Call incoming",
                    "Incoming call", 
                    data, 
                    PushNotificationType.Call);
            }

            return false;
        }
    }
}
