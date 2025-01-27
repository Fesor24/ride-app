using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Notifications;
using Soloride.Application.Abstractions.VoiceCall;
using Soloride.Application.Abstractions.Websocket;
using Soloride.Application.Models.WebSocket;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Shared.Constants;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.VoiceCall.NotifyRecipient
{
    internal sealed class NotifyRecipientCommandHandler :
        ICommandHandler<NotifyRecipientCommand>
    {
        private readonly IRideRepository _rideRepository;
        private readonly IRiderRepository _riderRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IDeviceNotificationService _deviceNotificationService;
        private readonly IWebSocketManager _webSocketManager;
        private readonly IVoiceService _voiceService;

        public NotifyRecipientCommandHandler(IRideRepository rideRepository,
            IRiderRepository riderRepository, IDriverRepository driverRepository,
            IDeviceNotificationService deviceNotificationService, IWebSocketManager webSocketManager,
            IVoiceService voiceService)
        {
            _rideRepository = rideRepository;
            _riderRepository = riderRepository;
            _driverRepository = driverRepository;
            _deviceNotificationService = deviceNotificationService;
            _webSocketManager = webSocketManager;
            _voiceService = voiceService;
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
                // notify driver
                if (!ride.DriverId.HasValue)
                    return Error.BadRequest("ride.notmatched", "Ride not matched");

                var rider = await _riderRepository
                    .GetAsync(ride.RiderId);

                if (rider is null) return true;

                string driverWebSocketKey = WebSocketKeys.Driver.Key(ride.DriverId.Value.ToString());

                //token for the driver...
                var (token, channel) = await _voiceService.GenerateAgoraAccessTokenAsync(ride.Id.ToString(), true);

                var callRouteMessage = new WebSocketResponse<object>
                {
                    Event = SocketEventConstants.ROUTE_CALL,
                    Payload = new 
                    { 
                        message = "Call routed",
                        createdAt = callInitiatedTime,
                        expiryInSeconds = 10,
                        rideId = ride.Id,
                        callerInfo = new
                        {
                            caller = "rider",
                            name = rider.FirstName + " " + rider.LastName,
                            profileImage = rider.ProfileImageUrl
                        },
                        callInfo = new
                        {
                            recipient = "driver",
                            token,
                            channel
                        }
                    }
                };

                var data = new Dictionary<string, string>
                {
                    {"expiryInSeconds", "10" },
                    {"createdAt", callInitiatedTime },
                };

                string message = Serialize.Object(callRouteMessage);

                await _webSocketManager.SendMessageAsync(driverWebSocketKey, callRouteMessage);

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

                // notify rider...
                var driver = await _driverRepository
                    .GetAsync(ride.DriverId!.Value);

                if (driver is null) return true;

                string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

                //token for the rider...
                var (token, channel) = await _voiceService.GenerateAgoraAccessTokenAsync(ride.Id.ToString(), false);

                var callRouteMessage = new WebSocketResponse<object>
                {
                    Event = SocketEventConstants.ROUTE_CALL,
                    Payload = new
                    {
                        message = "Call routed",
                        createdAt = callInitiatedTime,
                        expiryInSeconds = 10,
                        rideId = ride.Id,
                        callerInfo = new
                        {
                            caller = "driver",
                            name = driver.FirstName + " " + driver.LastName,
                            profileImage = driver.ProfileImageUrl
                        },
                        callInfo = new
                        {
                            recipient = "rider",
                            token,
                            channel
                        }
                    }
                };

                var data = new Dictionary<string, string>
                {
                    {"expiryInSeconds", "10" },
                    {"createdAt", callInitiatedTime },
                };

                string message = Serialize.Object(callRouteMessage);

                await _webSocketManager.SendMessageAsync(riderWebSocketKey, callRouteMessage);

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
