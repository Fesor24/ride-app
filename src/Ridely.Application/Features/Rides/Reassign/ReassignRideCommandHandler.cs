using Hangfire;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Referral;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Contracts.Models;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Rides.Reassign;
internal sealed class ReassignRideCommandHandler :
    ICommandHandler<ReassignRideCommand, ReassignRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideService _rideService;
    private readonly ICacheService _cacheService;
    private readonly IWebSocketManager _webSocketManager;
    private readonly IReferralService _referralService;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly PricingService _pricingService;

    public ReassignRideCommandHandler(IUnitOfWork unitOfWork, IRideService rideService,
        ICacheService cacheService, IWebSocketManager webSocketManager,
        IReferralService referralService, IRiderRepository riderRepository,
        IRideRepository rideRepository, IDriverRepository driverRepository, IRideLogRepository rideLogRepository,
        IPaymentRepository paymentRepository, PricingService pricingService)
    {
        _unitOfWork = unitOfWork;
        _rideService = rideService;
        _cacheService = cacheService;
        _webSocketManager = webSocketManager;
        _referralService = referralService;
        _riderRepository = riderRepository;
        _rideRepository = rideRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _paymentRepository = paymentRepository;
        _pricingService = pricingService;
    }

    public async Task<Result<ReassignRideResponse>> Handle(ReassignRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Category == RideCategory.Delivery)
            return Error.BadRequest("ridetype.invalid", "Deliveries can not be rerouted");

        if (ride.Status != RideStatus.InTransit)
            return Error.BadRequest("ridestatus.notinprogress", "Only ride in progress can be rerouted");

        ride.UpdateStatus(RideStatus.Reassigned, reassignReason: request.ReassignReason);

        RideLog rideLog = new(ride.Id, RideStatus.Reassigned);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        string rideObjectKey = RideKeys.Ride(ride.Id.ToString());

        await _cacheService.RemoveAsync(rideObjectKey);

        string driverMatchedKey = RideKeys.Matched(ride.DriverId!.Value.ToString());

        await _cacheService.RemoveAsync(driverMatchedKey);

        var driver = await _driverRepository
            .GetDetailsAsync(ride.DriverId!.Value);

        if (driver is null) return new ReassignRideResponse(false);

        driver.UpdateCompletedTrips();

        _driverRepository.Update(driver);

        string driverWebsocketKey = WebSocketKeys.Driver.Key(driver.Id.ToString());

        var driverRerouteObjectMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.DRIVER_REASSIGN,
            Payload = new
            {
                message = "Ride reassigned",
                stopLocation = new
                {
                    latitude = request.Source.Latitude,
                    longitutude = request.Source.Longitude
                }
            }
        };

        string driverRerouteMessage = Serialize.Object(driverRerouteObjectMessage);

        await _webSocketManager.SendMessageAsync(driverWebsocketKey, driverRerouteMessage);

        // todo: charge for previous ride...
        // communicate price to driver via websocket...same with rider...
        //BackgroundJob.Enqueue(() => paymentService.ChargeRideAsync(ride.Id));

        var res = await _rideService.ComputeEstimatedFare(new Domain.Models.Location
        {
            Latitude = request.Source.Latitude,
            Longitude = request.Source.Longitude
        }, ride.GetCoordinates(ride.DestinationCordinates));

        if (res.IsFailure) return res.Error;

        long estimatedFare = _pricingService.FormatPrice(res.Value.EstimatedFare);

        Payment ridePayment = new(
            estimatedFare,
            Ulid.NewUlid(),
            ride.Payment.Method,
            ride.Payment.PaymentCardId
            );

        //await _unitOfWork.PaymentRepository.AddAsync(ridePayment);

        BackgroundJob.Enqueue(() => _referralService.RewardsAfterRidersFirstCompletedRide(ride.RiderId));

        var destinationCoordinates = ride.GetCoordinates(ride.DestinationCordinates);

        await _paymentRepository.AddAsync(ridePayment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Ride newRide = Ride.CreateRide(
            ride.RiderId,
            estimatedFare,
            0,
            request.Source.Latitude,
            request.Source.Longitude,
            destinationCoordinates.Latitude,
            destinationCoordinates.Longitude,
            ridePayment.Id,
            res.Value.DistanceInMeters,
            request.SourceAddress,
            ride.DestinationAddress,
            res.Value.DurationInSeconds,
            ride.HaveConversation,
            ride.MusicGenre,
            RideStatus.Requested,
            ride.Id
            );   

        await _rideRepository.AddAsync(newRide);

        RideLog newRideLog = new(newRide.Id, RideStatus.Requested);

        await _rideLogRepository.AddAsync(newRideLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string rideMatchKey = RideKeys.RideNotMatched(newRide.Id);

        await _cacheService.SetAsync(rideMatchKey, 1.ToString(), TimeSpan.FromMinutes(120));

        RideObject rideObject = new(
            newRide.Id,
            newRide.SourceAddress,
            newRide.DestinationAddress,
            newRide.MusicGenre.ToString(),
            newRide.HaveConversation,
            newRide.EstimatedFare,
            newRide.Payment.Method.ToString()
            );

        var matchResult = await _rideService.SendRequestToDriversAsync(
            request.Source, rideObject, ride.Rider, [ride.DriverId!.Value],
            ride.Category);

        string riderRequestKey = RideKeys.RiderRequestRide(ride.RiderId);

        if (!matchResult)
        {
            await _cacheService.RemoveAsync(riderRequestKey);

            var rider = await _riderRepository
            .GetAsync(ride.RiderId);

            if (rider is not null)
            {
                // todo: review if we will be warning the driver first in a situation where there is no driver
                rider.UpdateStatus(RiderStatus.Online, null);

                _riderRepository.Update(rider);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            await _cacheService.SetAsync(riderRequestKey, newRide.Id.ToString(), 
                TimeSpan.FromMinutes(30000));
        }

        return new ReassignRideResponse(matchResult);
    }
}
