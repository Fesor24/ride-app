using Hangfire;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Rides.StartRide;
internal sealed class StartRideCommandHandler :
    ICommandHandler<StartRideCommand, StartRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebSocketManager _webSocketManager;
    private readonly IRideRepository _rideRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IPaymentService _paymentService;
    private readonly IDeviceNotificationService _deviceNotificationService;
    private readonly IPaystackService _paystackService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRiderRepository _riderRepository;

    public StartRideCommandHandler(IUnitOfWork unitOfWork,
        IWebSocketManager webSocketManager, IRideRepository rideRepository,
        IRideLogRepository rideLogRepository, IDriverRepository driverRepository, 
        IPaymentService paymentService, IDeviceNotificationService deviceNotificationService,
        IPaystackService paystackService, IPaymentRepository paymentRepository,
        IRiderRepository riderRepository)
    {
        _unitOfWork = unitOfWork;
        _webSocketManager = webSocketManager;
        _rideRepository = rideRepository;
        _rideLogRepository = rideLogRepository;
        _driverRepository = driverRepository;
        _paymentService = paymentService;
        _deviceNotificationService = deviceNotificationService;
        _paystackService = paystackService;
        _paymentRepository = paymentRepository;
        _riderRepository = riderRepository;
    }

    public async Task<Result<StartRideResponse>> Handle(StartRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Status != RideStatus.Arrived)
            return Error.BadRequest("ride.drivernotarrived", "Driver not at pickup");

        ride.Driver.UpdateStatus(DriverStatus.InTrip, ride.Id);

        ride.UpdateStatus(RideStatus.InTransit);

        RideLog rideLog = new(ride.Id, RideStatus.InTransit);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        _driverRepository.Update(ride.Driver);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var source = ride.GetCoordinates(ride.SourceCordinates);

        var destination = ride.GetCoordinates(ride.DestinationCordinates);

        string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());
        var startRideMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.RIDE_START,
            Payload = new
            {
                message = "Ride started"
            }
        };

        string requestMessage = Serialize.Object(startRideMessage);
        await _webSocketManager.SendMessageAsync(riderWebSocketKey, requestMessage);

        // todo: refactor code in places where we using background service...
        // see if db calls can be reduced by passing the domain object itself...
        BackgroundJob.Enqueue(() => HandleCardTripPayment(ride, 
            ride.Rider.DeviceTokenId ?? "",
            ride.Driver.DeviceTokenId ?? "",
            cancellationToken));

        return new StartRideResponse
        {
            Destination = new StartRideResponse.RideLocation
            {
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                Address = ride.DestinationAddress
            },
            Source = new StartRideResponse.RideLocation
            {
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Address = ride.SourceAddress
            },
            PaymentMethod = ride.Payment.Method,
            RideConversation = ride.HaveConversation,
            MusicGenre = ride.MusicGenre
        };
    }

    // todo: review implementation...
    public async Task<Result> HandleCardTripPayment(Domain.Rides.Ride ride, 
        string riderDeviceTokenId, string driverDeviceTokenId,
        CancellationToken cancellationToken)
    {
        if (ride.Payment is null) return Result.Success();

        if (ride.Payment.Method != PaymentMethod.Card) return Result.Success();

        var result = await _paymentService.ProcessCardTripPaymentAsync(ride, cancellationToken);

        if (result.IsSuccessful)
        {
            int maximumDurationInSeconds = 12 * 60;// 12 mins

            if(ride.EstimatedDurationInSeconds <= maximumDurationInSeconds)
                maximumDurationInSeconds = ride.EstimatedDurationInSeconds - (2 * 60);// reduce by 2 mins...

            BackgroundJob.Schedule(() => CheckPaymentUpdateMethodToCashIfFail(ride.Id, riderDeviceTokenId, driverDeviceTokenId),
                DateTimeOffset.UtcNow.AddSeconds(maximumDurationInSeconds));

            return Result.Success();
        }

        await HandleRidePaymentFailure(result.Error, ride.Id, riderDeviceTokenId, driverDeviceTokenId);

        return Result.Success();
    }

    public async Task HandleRidePaymentFailure(Error error, long rideId,
        string riderDeviceTokenId, string driverDeviceTokenId)
    {
        // note: based on this...mobile notify user that ride payment has been updated to cash...
        var paymentFailMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.CARD_TRIP_PAYMENT_FAILURE,
            Payload = new
            {
                message = "An error occurred during processing payment for this ride",
                details = error.Message
            }
        };

        var ride = await _rideRepository.GetAsync(rideId);

        string paymentFailureMessage = Serialize.Object(paymentFailMessage);

        string riderWebSocketKey = WebSocketKeys.Rider.Key(ride!.RiderId.ToString());
        string driverWebSocketKey = WebSocketKeys.Driver.Key(ride.DriverId!.Value.ToString());

        await _webSocketManager.SendMessageAsync(riderWebSocketKey, paymentFailureMessage);
        await _webSocketManager.SendMessageAsync(driverWebSocketKey, paymentFailureMessage);

        if (!string.IsNullOrWhiteSpace(riderDeviceTokenId))
        {
            Dictionary<string, string> data = new()
            {

            };

            await _deviceNotificationService.PushAsync(
                riderDeviceTokenId,
                "Card payment failed",
                "Payment updated to cash",
                data, PushNotificationType.CardTripPaymentFailure);
        }

        if (!string.IsNullOrWhiteSpace(driverDeviceTokenId))
        {
            Dictionary<string, string> data = new()
            {

            };

            await _deviceNotificationService.PushAsync(
                driverDeviceTokenId,
                "Card payment failed",
                "Payment updated to cash",
                data, PushNotificationType.CardTripPaymentFailure);
        }

        ride.Payment.UpdatePaymentMethod(PaymentMethod.Cash);

        _rideRepository.Update(ride);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CheckPaymentUpdateMethodToCashIfFail(long rideId, string riderDeviceTokenId,
        string driverDeviceTokenId)
    {
        var ride = await _rideRepository.GetAsync(rideId);

        if (ride is null) return;

        if (ride.Payment.Method == PaymentMethod.Cash) return;

        if (ride.Payment.Status == PaymentStatus.Success) return;

        var verificationResult = await _paystackService.VerifyAsync(ride.Payment.Reference.ToString());

        if (verificationResult.IsSuccessful && verificationResult.Value.Status && 
            verificationResult.Value.Data.Status == "success")
        {
            ride.Payment.UpdateStatus(PaymentStatus.Success);

            _paymentRepository.Update(ride.Payment);

            await _unitOfWork.SaveChangesAsync();

            return;
        }

        var rider = await _riderRepository.GetAsync(ride.RiderId);

        if (rider is null) return;

        await HandleRidePaymentFailure(
            new Error("payment.failure", "Payment failed"),
            rideId, riderDeviceTokenId, driverDeviceTokenId);
    }
}
