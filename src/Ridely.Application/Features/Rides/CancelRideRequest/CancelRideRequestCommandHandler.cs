using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.CancelRideRequest;
internal sealed class CancelRideRequestCommandHandler :
    ICommandHandler<CancelRideRequestCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPushNotificationService _deviceNotificationService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentService _paymentService;
    private readonly IHubContext<RideHub> _rideHubContext;

    public CancelRideRequestCommandHandler(IUnitOfWork unitOfWork,
        ICacheService cacheService, IPushNotificationService deviceNotificationService,
        IRideRepository rideRepository, IRiderRepository riderRepository, IDriverRepository driverRepository,
        IRideLogRepository rideLogRepository, IPaymentService paymentService, IHubContext<RideHub> rideHubContext)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _deviceNotificationService = deviceNotificationService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _paymentService = paymentService;
        _rideHubContext = rideHubContext;
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

        RideLog rideLog = new(ride.Id, RideLogEvent.Cancelled);

        await _rideLogRepository.AddAsync(rideLog);

        if (ride.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetAsync(ride.DriverId.Value);

            driver!.UpdateStatus(DriverStatus.Online);

            await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(driver.Id))
                .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
                {
                    Update = ReceiveRideUpdate.Cancelled,
                    Data = JsonSerializer.Serialize(new
                    {
                        Message = "Ride cancelled"
                    })
                }, cancellationToken);

            //string driverWebSocketKey = WebSocketKeys.Driver.Key(driver.Id.ToString());

            //var cancellationMessage = WebSocketMessage<object>.Create(
            //    SocketEventConstants.RIDE_CANCELLATION,
            //    new
            //    {
            //        message = "Ride request cancelled"
            //    });

            //await _webSocketManager.SendMessageAsync(driverWebSocketKey, cancellationMessage);

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
                    "Ride has been cancelled",
                    new Dictionary<string, string>(),
                    PushNotificationType.RideCancelled);
            }

            await AddDriverToCancelled(ride.RiderId, driver.Id);
        }

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        if (rider is not null)
        {
            if(request.CancelledBy == UserType.Driver)
            {
                await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(rider.Id))
                  .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
                  {
                      Update = ReceiveRideUpdate.Cancelled,
                      Data = JsonSerializer.Serialize(new
                      {
                          Message = "Ride cancelled"
                      })
                  }, cancellationToken);

                if (!string.IsNullOrWhiteSpace(rider.DeviceTokenId))
                {
                    await _deviceNotificationService.PushAsync(
                        rider.DeviceTokenId,
                        "Ride cancelled",
                        "Ride has been cancelled",
                        new Dictionary<string, string>(),
                        PushNotificationType.RideCancelled);
                }
            }

            rider.UpdateStatus(RiderStatus.Online);

            _riderRepository.Update(rider);
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }

    private static Result<bool> CanBeCancelled(RideStatus status) =>
        status switch
        {
            RideStatus.Unknown => Error.BadRequest("ridestatus.unknown", "Ride status unknown"),
            RideStatus.Requested => true,
            RideStatus.Matched => true,
            RideStatus.Arrived => true,
            RideStatus.Started => Error.BadRequest("ride.inprogress", "Ride in progress and can not be cancelled"),
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
