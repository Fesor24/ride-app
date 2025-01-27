using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Rides.DriverSourceArrival;
internal sealed class DriverSourceArrivalCommandHandler :
    ICommandHandler<DriverSourceArrivalCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebSocketManager _webSocketManager;
    private readonly IDeviceNotificationService _deviceNotificationService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideLogRepository _rideLogRepository;

    public DriverSourceArrivalCommandHandler(IUnitOfWork unitOfWork, IWebSocketManager webSocketManager,
        IDeviceNotificationService deviceNotificationService, IRideRepository rideRepository,
        IRiderRepository riderRepository, IRideLogRepository rideLogRepository)
    {
        _unitOfWork = unitOfWork;
        _webSocketManager = webSocketManager;
        _deviceNotificationService = deviceNotificationService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _rideLogRepository = rideLogRepository;
    }

    public async Task<Result<bool>> Handle(DriverSourceArrivalCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        ride.UpdateStatus(RideStatus.Arrived);

        RideLog rideLog = new(ride.Id, RideStatus.Arrived);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        var arrivalMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.DRIVER_SOURCE_ARRIVAL,
            Payload = new { message = "Your ride is here" }
        };

        var rider = await _riderRepository.GetAsync(ride.RiderId);

        if (rider is not null)
        {
            if (!string.IsNullOrWhiteSpace(rider.DeviceTokenId))
            {
                Dictionary<string, string> data = new()
                {
                    {"rideId", ride.Id.ToString()}
                };

                await _deviceNotificationService.PushAsync(
                    rider.DeviceTokenId,
                    "Driver arrived",
                    "Your ride is here",
                    data, PushNotificationType.DriverArrived);
            }
        }

        string message = Serialize.Object(arrivalMessage);

        await _webSocketManager.SendMessageAsync(riderWebSocketKey, message);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}