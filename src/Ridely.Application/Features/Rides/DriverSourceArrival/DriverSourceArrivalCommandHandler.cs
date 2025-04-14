using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Settings;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.DriverSourceArrival;
internal sealed class DriverSourceArrivalCommandHandler :
    ICommandHandler<DriverSourceArrivalCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPushNotificationService _deviceNotificationService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentDetailRepository _paymentDetailRepository;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly ApplicationSettings _applicationSettings;

    public DriverSourceArrivalCommandHandler(IUnitOfWork unitOfWork,
        IPushNotificationService deviceNotificationService, IRideRepository rideRepository,
        IRiderRepository riderRepository, IRideLogRepository rideLogRepository,
        IPaymentDetailRepository paymentDetailRepository,
        IHubContext<RideHub> rideHubContext, IOptions<ApplicationSettings> appSettings)
    {
        _unitOfWork = unitOfWork;
        _deviceNotificationService = deviceNotificationService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _rideLogRepository = rideLogRepository;
        _paymentDetailRepository = paymentDetailRepository;
        _rideHubContext = rideHubContext;
        _applicationSettings = appSettings.Value;
    }

    public async Task<Result<bool>> Handle(DriverSourceArrivalCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        ride.UpdateStatus(RideStatus.Arrived);

        RideLog rideLog = new(ride.Id, RideLogEvent.Arrived);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(ride.RiderId))
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                Update = ReceiveRideUpdate.DriverArrived,
                Data = JsonSerializer.Serialize(new
                {
                    Message = "Your driver is here",
                    WaitingTimeInMinutes = _applicationSettings.FreeWaitingTimeInMins
                })
            });

        var estimatedChargePaymentDetail = await _paymentDetailRepository.GetAsync(PaymentFor.EstimatedCharge, ride.PaymentId);

        if(estimatedChargePaymentDetail is null)
        {
            estimatedChargePaymentDetail = new(Ulid.NewUlid(), ride.PaymentId, PaymentFor.EstimatedCharge, ride.EstimatedFare);

            await _paymentDetailRepository.AddAsync(estimatedChargePaymentDetail);
        }

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        //var arrivalMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.DRIVER_SOURCE_ARRIVAL,
        //    new
        //    {
        //        message = "Your ride is here"
        //    });

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

        //await _webSocketManager.SendMessageAsync(riderWebSocketKey, arrivalMessage);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}