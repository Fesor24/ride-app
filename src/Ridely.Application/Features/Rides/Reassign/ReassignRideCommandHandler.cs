using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Referral;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Hubs;
using Ridely.Contracts.Models;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.Reassign;
internal sealed class ReassignRideCommandHandler :
    ICommandHandler<ReassignRideCommand, ReassignRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideService _rideService;
    private readonly ICacheService _cacheService;
    private readonly IReferralService _referralService;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentDetailRepository _paymentDetailRepository;
    private readonly PricingService _pricingService;
    private readonly IHubContext<RideHub> _rideHubContext;

    public ReassignRideCommandHandler(IUnitOfWork unitOfWork, IRideService rideService,
        ICacheService cacheService,
        IReferralService referralService, IRiderRepository riderRepository,
        IRideRepository rideRepository, IDriverRepository driverRepository, IRideLogRepository rideLogRepository,
        IPaymentRepository paymentRepository, IPaymentDetailRepository paymentDetailRepository, PricingService pricingService,
        IHubContext<RideHub> rideHubContext)
    {
        _unitOfWork = unitOfWork;
        _rideService = rideService;
        _cacheService = cacheService;
        _referralService = referralService;
        _riderRepository = riderRepository;
        _rideRepository = rideRepository;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _paymentRepository = paymentRepository;
        _paymentDetailRepository = paymentDetailRepository;
        _pricingService = pricingService;
        _rideHubContext = rideHubContext;
    }

    public async Task<Result<ReassignRideResponse>> Handle(ReassignRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Category == RideCategory.Delivery)
            return Error.BadRequest("ridetype.invalid", "Deliveries can not be rerouted");

        if (ride.Status != RideStatus.Started)
            return Error.BadRequest("ridestatus.notinprogress", "Only rides in progress can be rerouted");

        ride.UpdateStatus(RideStatus.Reassigned, reassignReason: request.ReassignReason);

        RideLog rideLog = new(ride.Id, RideLogEvent.Reassigned);

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

        driver.UpdateStatus(DriverStatus.Online);

        _driverRepository.Update(driver);

        await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(driver.Id))
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                update = ReceiveRideUpdate.Reassigned,
                data = JsonSerializer.Serialize(new
                {
                    stop = new
                    {
                        latitude = request.Source.Latitude,
                        longitude = request.Source.Longitude
                    }
                })
            });

        //string driverWebsocketKey = WebSocketKeys.Driver.Key(driver.Id.ToString());

        //var driverRerouteMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.DRIVER_REASSIGN,
        //    new
        //    {
        //        message = "Ride reassigned",
        //        stopLocation = new
        //        {
        //            latitude = request.Source.Latitude,
        //            longitutude = request.Source.Longitude
        //        }
        //    });

        //await _webSocketManager.SendMessageAsync(driverWebsocketKey, driverRerouteMessage);

        // todo: charge for previous ride...
        // todo: review and sort payment globally...
        // use different service for payment??
        // Questions for CEO on paymen
        // communicate price to driver via websocket...same with rider...
        //BackgroundJob.Enqueue(() => paymentService.ChargeRideAsync(ride.Id));

        var res = await _rideService.ComputeEstimatedFare(new Domain.Models.Location
        {
            Latitude = request.Source.Latitude,
            Longitude = request.Source.Longitude
        }, ride.GetCoordinates(ride.DestinationCordinates));

        if (res.IsFailure) return res.Error;

        long estimatedFare = _pricingService.ConvertPriceInDecimalToLong(res.Value.EstimatedFare);

        Payment ridePayment = new(
            ride.Payment.Method,
            ride.Payment.PaymentCardId
            );

        await _paymentRepository.AddAsync(ridePayment);

        await _unitOfWork.SaveChangesAsync();

        PaymentDetail paymentDetail = new(Ulid.NewUlid(), ridePayment.Id, PaymentFor.EstimatedCharge, ride.EstimatedFare);

        await _paymentDetailRepository.AddAsync(paymentDetail);

        BackgroundJob.Enqueue(() => _referralService.RewardsAfterRidersFirstCompletedRide(ride.RiderId));
        BackgroundJob.Enqueue(() => _referralService.RewardsAfterDriversFirstCompletedRide(ride.DriverId!.Value));

        var destinationCoordinates = ride.GetCoordinates(ride.DestinationCordinates);

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

        RideLog newRideLog = new(newRide.Id, RideLogEvent.Requested);

        await _rideLogRepository.AddAsync(newRideLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string rideMatchKey = RideKeys.RideNotMatched(newRide.Id);

        await _cacheService.SetAsync(rideMatchKey, 1.ToString(), TimeSpan.FromMinutes(120));

        RideObject rideObject = new(
            newRide.Id,
            newRide.SourceAddress,
            newRide.DestinationAddress,
            [],// todo: handle waypoints...
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
