using Hangfire;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Payment;
using Soloride.Application.Abstractions.Referral;
using Soloride.Application.Abstractions.Websocket;
using Soloride.Application.Models.WebSocket;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Services;
using Soloride.Shared.Constants;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.Rides.EndRide;
internal sealed class EndRideCommandHandler :
    ICommandHandler<EndRideCommand, EndRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPaymentService _paymentService;
    private readonly IWebSocketManager _webSocketManager;
    private readonly IReferralService _referralService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRiderWalletRepository _riderWalletRepository;
    private readonly IDriverWalletRepository _driverWalletRepository;

    public EndRideCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService,
        IPaymentService paymentService, IWebSocketManager webSocketManager,
        IReferralService referralService, IRideRepository rideRepository,
        IRiderRepository riderRepository, IDriverRepository driverRepository,
        IRideLogRepository rideLogRepository, ISettingsRepository settingsRepository,
        IPaymentRepository paymentRepository, IRiderWalletRepository riderWalletRepository,
        IDriverWalletRepository driverWalletRepository)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _paymentService = paymentService;
        _webSocketManager = webSocketManager;
        _referralService = referralService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _settingsRepository = settingsRepository;
        _paymentRepository = paymentRepository;
        _riderWalletRepository = riderWalletRepository;
        _driverWalletRepository = driverWalletRepository;
    }

    public async Task<Result<EndRideResponse>> Handle(EndRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Status == RideStatus.Completed)
            return Error.BadRequest("ride.completed", "Ride completed");

        if (ride.Status != RideStatus.InTransit)
            return Error.BadRequest("ride.notinprogress", "Only rides in progress can be ended");

        var driver = await _driverRepository
                 .GetAsync(ride.DriverId!.Value);

        if (driver is not null)
        {
            driver.UpdateCompletedTrips();

            driver.UpdateStatus(Domain.Drivers.DriverStatus.Online);

            _driverRepository.Update(driver);
        }

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        if (rider is not null)
        {
            rider.UpdateStatus(RiderStatus.Online);

            _riderRepository.Update(rider);
        }

        string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        ride.UpdateStatus(RideStatus.Completed);

        RideLog rideLog = new(ride.Id, RideStatus.Completed);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        var payment = await _paymentRepository.GetAsync(ride.PaymentId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await ClearCache(ride.Id, ride.DriverId!.Value);

        long rideAmount = ride.EstimatedFare;

        bool ridePaidFor = false;

        if (ride.Payment.Status == PaymentStatus.Success) ridePaidFor = true;

        (long rideFare, long amountDue) = await _paymentService
            .ProcessPaymentAndDriverCommissionAsync(driver!, ride, cancellationToken);

        if (amountDue == 0) ridePaidFor = true;

        var endRideMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.RIDE_END,
            Payload = new
            {
                message = "Ride ended",
                fare = rideFare,
                fareDueByRider = amountDue,
                paid = ridePaidFor,
            }
        };

        string requestMessage = Serialize.Object(endRideMessage);
        await _webSocketManager.SendMessageAsync(riderWebSocketKey, requestMessage);

        BackgroundJob.Enqueue(() => _referralService.RewardsAfterRidersFirstCompletedRide(ride.RiderId));

        return new EndRideResponse
        {
            Fare = rideFare,
            FareDueByRider = amountDue,
            Source = ride.SourceAddress,
            Destination = ride.DestinationAddress,
            Paid = ridePaidFor
        };
    }

    private async Task ClearCache(long rideId, long driverId)
    {
        string rideObjectKey = RideKeys.Ride(rideId.ToString());

        await _cacheService.RemoveAsync(rideObjectKey);

        string driverMatchedKey = RideKeys.Matched(driverId.ToString());

        await _cacheService.RemoveAsync(driverMatchedKey);
    }
}
