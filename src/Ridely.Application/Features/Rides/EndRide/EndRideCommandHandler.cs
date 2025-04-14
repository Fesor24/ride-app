using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Referral;
using Ridely.Application.Abstractions.Settings;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.EndRide;
internal sealed class EndRideCommandHandler :
    ICommandHandler<EndRideCommand, EndRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPaymentService _paymentService;
    private readonly IReferralService _referralService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly ApplicationSettings _applicationSettings;

    public EndRideCommandHandler(IUnitOfWork unitOfWork, ICacheService cacheService,
        IPaymentService paymentService,
        IReferralService referralService, IRideRepository rideRepository,
        IRiderRepository riderRepository, IDriverRepository driverRepository,
        IRideLogRepository rideLogRepository,
        IPaymentRepository paymentRepository, IHubContext<RideHub> rideHubContext,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _paymentService = paymentService;
        _referralService = referralService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _paymentRepository = paymentRepository;
        _rideHubContext = rideHubContext;
        _applicationSettings = applicationSettings.Value;
    }

    public async Task<Result<EndRideResponse>> Handle(EndRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Status == RideStatus.Completed)
            return Error.BadRequest("ride.completed", "Ride completed");

        if (ride.Status != RideStatus.Started)
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

        RideLog rideLog = new(ride.Id, RideLogEvent.Completed);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        var payment = await _paymentRepository.GetAsync(ride.PaymentId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await ClearCache(ride.Id, ride.DriverId!.Value);

        long rideAmount = ride.EstimatedFare;

        List<RideStatus> relevantRideLogStatuses = [RideStatus.Arrived, RideStatus.Started];

        var paymentResponse = await _paymentService
            .ProcessPaymentAndDriverCommissionAsync(driver!, ride, cancellationToken);

        string[] waypoints = ride.WaypointAddresses.Split("%%");

        //var endRideMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDE_END,
        //    new
        //    {
              
        //    });  

        //await _webSocketManager.SendMessageAsync(riderWebSocketKey, endRideMessage);

        BackgroundJob.Enqueue(() => _referralService.RewardsAfterRidersFirstCompletedRide(ride.RiderId));
        BackgroundJob.Enqueue(() => _referralService.RewardsAfterDriversFirstCompletedRide(ride.DriverId!.Value));

        await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(ride.RiderId))
        .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
        {
            Update = ReceiveRideUpdate.Ended,
            Data = JsonSerializer.Serialize(new
            {
                TotalFareAmount = paymentResponse.TotalRideFare,
                FareOutstanding = paymentResponse.AmountOutstanding,
                Source = ride.SourceAddress,
                Destination = ride.DestinationAddress,
                Waypoints = waypoints,
                ride.Payment.DiscountInPercent,
            })
        }, cancellationToken);

        return new EndRideResponse
        {
            TotalFareAmount = paymentResponse.TotalRideFare,
            FareOutstanding = paymentResponse.AmountOutstanding,
            Source = ride.SourceAddress,
            Destination = ride.DestinationAddress,
            Waypoints = waypoints,
            DiscountInPercent = ride.Payment.DiscountInPercent,
            WaitingTimeCharge = paymentResponse.WaitingTimeCharge
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
