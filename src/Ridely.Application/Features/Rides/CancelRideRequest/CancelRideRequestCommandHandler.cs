using System.Text.Json;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Rides.CancelRideRequest;
internal sealed class CancelRideRequestCommandHandler :
    ICommandHandler<CancelRideRequestCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebSocketManager _webSocketManager;
    private readonly ICacheService _cacheService;
    private readonly IDeviceNotificationService _deviceNotificationService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentService _paymentService;

    public CancelRideRequestCommandHandler(IUnitOfWork unitOfWork, IWebSocketManager webSocketManager,
        ICacheService cacheService, IDeviceNotificationService deviceNotificationService,
        IRideRepository rideRepository, IRiderRepository riderRepository, IDriverRepository driverRepository,
        IRideLogRepository rideLogRepository, IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _webSocketManager = webSocketManager;
        _cacheService = cacheService;
        _deviceNotificationService = deviceNotificationService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _paymentService = paymentService;
    }

    public async Task<Result<bool>> Handle(CancelRideRequestCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetAsync(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        // if invoked by system, we check if the payment has been updated to cash
        if (request.SystemInvoked == true && ride.Payment.Method == PaymentMethod.Cash) return true;

        var result = CanBeCancelled(ride.Status);

        if (result.IsFailure)
            return result.Error;

        string rideCancelledKey = RideKeys.RideCancelled(ride.Id.ToString());

        await _cacheService.SetAsync(rideCancelledKey, ride.Id.ToString(), expiry: TimeSpan.FromMinutes(30));

        string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

        await _cacheService.RemoveAsync(rideNotMatchedKey);

        ride.UpdateStatus(RideStatus.Cancelled, cancellationReason: request.CancellationReason);

        _rideRepository.Update(ride);

        RideLog rideLog = new(ride.Id, RideStatus.Cancelled);

        await _rideLogRepository.AddAsync(rideLog);

        if (ride.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetAsync(ride.DriverId.Value);

            driver!.UpdateStatus(DriverStatus.Online);

            string driverWebSocketKey = WebSocketKeys.Driver.Key(driver.Id.ToString());

            var cancellationMessage = new WebSocketResponse<object>
            {
                Event = SocketEventConstants.RIDE_CANCELLATION,
                Payload = new { message = "Ride request cancelled" }
            };

            string message = Serialize.Object(cancellationMessage);

            await _webSocketManager.SendMessageAsync(driverWebSocketKey, message);

            string driverMatchedKey = RideKeys.Matched(driver.Id.ToString());

            string rideObjKey = RideKeys.Ride(ride.Id.ToString());

            await _cacheService.RemoveAsync(rideObjKey);
            await _cacheService.RemoveAsync(driverMatchedKey);

            _driverRepository.Update(driver);

            if (!string.IsNullOrWhiteSpace(driver.DeviceTokenId))
            {
                await _deviceNotificationService.PushAsync(
                    driver.DeviceTokenId,
                    "Ride cancelled",
                    "Ride request cancelled",
                    new Dictionary<string, string>(),
                    PushNotificationType.RideCancelled);
            }

            await AddDriverToCancelled(ride.RiderId, driver.Id);
        }

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        if (rider is not null)
        {
            rider.UpdateStatus(RiderStatus.Online);

            _riderRepository.Update(rider);
        }

        await RefundRiderIfNecessary(ride);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }

    private async Task RefundRiderIfNecessary(Ride ride)
    {
        if (!(ride.Payment.Method == PaymentMethod.Card && ride.Payment.Status == PaymentStatus.Success))
            return;

        await _paymentService.RefundAsync(ride, ride.Payment.Reference);
    }

    private static Result<bool> CanBeCancelled(RideStatus status) =>
        status switch
        {
            RideStatus.Unknown => Error.BadRequest("ridestatus.unknown", "Ride status unknown"),
            RideStatus.Requested => true,
            RideStatus.Matched => true,
            RideStatus.Arrived => true,
            RideStatus.InTransit => Error.BadRequest("ride.inprogress", "Ride in progress and can not be cancelled"),
            RideStatus.Completed => Error.BadRequest("ride.completed", "Ride has been completed and can not be cancelled"),
            RideStatus.Cancelled => Error.BadRequest("ride.cancelled", "Ride cancelled already"),
            _ => Error.BadRequest("invalid.ridestatus", "Ride status is invalid")
        };

    private async Task AddDriverToCancelled(long riderId, long driverId)
    {
        string key = RideKeys.DriversCancelled(riderId.ToString());

        var driversCancelled = await _cacheService.GetAsync(key);

        if (string.IsNullOrWhiteSpace(driversCancelled))
        {
            List<CancelledDrivers> cancelledDrivers = [
                new(driverId, DateTime.UtcNow.AddMinutes(3))
                ];

            await _cacheService.SetAsync(key, JsonSerializer.Serialize(cancelledDrivers), TimeSpan.FromMinutes(4));
        }
        else
        {
            var cancelledDrivers = JsonSerializer.Deserialize<List<CancelledDrivers>>(driversCancelled) ?? [];

            cancelledDrivers.Add(new(driverId, DateTime.UtcNow.AddMinutes(3)));

            await _cacheService.SetAsync(key, JsonSerializer.Serialize(cancelledDrivers), TimeSpan.FromMinutes(4));
        }
    }
}
