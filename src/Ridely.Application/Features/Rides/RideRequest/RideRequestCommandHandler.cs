using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Hubs;
using Ridely.Contracts.Models;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Models;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Domain.Transactions;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.RideRequest;
internal sealed class RideRequestCommandHandler:
    ICommandHandler<RideRequestCommand, RideRequestResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideService _rideService;
    private readonly ICacheService _cacheService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IPaymentCardRepository _paymentCardRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IPaymentDetailRepository _paymentDetailRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IHubContext<RideHub> _rideHubContext;

    public RideRequestCommandHandler(IUnitOfWork unitOfWork, IRideService rideService, 
        ICacheService cacheService, IRideRepository rideRepository, IRiderRepository riderRepository,
        IPaymentCardRepository paymentCardRepository, IPaymentRepository paymentRepository,
        IRideLogRepository rideLogRepository, IPaymentDetailRepository paymentDetailRepository,
        ISettingsRepository settingsRepository, IHubContext<RideHub> rideHubContext)
    {
        _unitOfWork = unitOfWork;
        _rideService = rideService;
        _cacheService = cacheService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _paymentCardRepository = paymentCardRepository;
        _paymentRepository = paymentRepository;
        _rideLogRepository = rideLogRepository;
        _paymentDetailRepository = paymentDetailRepository;
        _settingsRepository = settingsRepository;
        _rideHubContext = rideHubContext;
    }

    public async Task<Result<RideRequestResponse>> Handle(RideRequestCommand request, CancellationToken cancellationToken)
    {
        if ((int)request.PaymentMethod < 1)
            return Error.BadRequest("payment.method", "Select valid payment method");

        bool validPaymentMethodEnum = Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod);

        if (!validPaymentMethodEnum) return Error.BadRequest("payment.method", "Select valid payment method");

        if ((int)request.RideCategory < 1)
            return Error.BadRequest("ride.category", "Select valid ride category");

        bool validRideCatEnum = Enum.IsDefined(typeof(RideCategory), request.RideCategory);

        if (!validRideCatEnum) return Error.BadRequest("ride.category", "Select valid ride category");

        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (!(ride.Status == RideStatus.FareEstimate || ride.Status == RideStatus.Requested))
            return Error.BadRequest("ride.invalidstatus", "Only rides with 'FareEstimate' or 'Requested' status");

        var rider = await _riderRepository
            .GetAsync(ride.RiderId);

        if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

        if (rider.CurrentRideId.HasValue && rider.CurrentRideId.Value != default)
            return Error.BadRequest("riderequest.duplicate", "Duplicate ride request");

        if (rider.Status == RiderStatus.InTrip)
            return Error.BadRequest("rider.intrip", "Rider is in a current trip and can not make a ride request");

        if (request.PaymentMethod == PaymentMethod.Card)
        {
            if (request.PaymentCardId is null)
                return Error.BadRequest("invalid.paymentcardid", "Select payment card");

            var paymentCard = await _paymentCardRepository
                .GetAsync(request.PaymentCardId.Value);

            if (paymentCard is null)
                return Error.NotFound("paymentcard.notfound", "Payment card not found");
        }

        Payment? ridePayment = await _paymentRepository.GetAsync(ride.PaymentId);

        if (ridePayment is null)
        {
            ridePayment = new(request.PaymentMethod, request.PaymentCardId);

            await _paymentRepository.AddAsync(ridePayment);

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            ridePayment.UpdatePaymentMethod(request.PaymentMethod, request.PaymentCardId);

            _paymentRepository.Update(ridePayment);
        }

        long initialCharge;

        if (request.RideCategory == RideCategory.Delivery) initialCharge = ride.EstimatedDeliveryFare;

        else
        {
            if (request.RideCategory == RideCategory.CabEconomy)
                initialCharge = ride.EstimatedFare;

            else
            {
                var settings = await _settingsRepository.GetAllAsync();

                int premiumCabPrice = settings.First().PremiumCab;

                initialCharge = ride.EstimatedFare + premiumCabPrice;
            }
        }

        PaymentDetail paymentDetail = new(
            Ulid.NewUlid(),
            ridePayment.Id,
            PaymentFor.EstimatedCharge,
            initialCharge
            );

        await _paymentDetailRepository.AddAsync(paymentDetail);

        bool increaseSearchRadius = ride.Status == RideStatus.Requested;

        ride.UpdateRideRequest(
            request.RideCategory,
            request.RideConversation,
            request.MusicGenre);

        _rideRepository.Update(ride);

        RideLog rideLog = new(
            ride.Id,
            RideLogEvent.Requested);

        await _rideLogRepository.AddAsync(rideLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

        await _cacheService.SetAsync(rideNotMatchedKey, 1.ToString(), TimeSpan.FromMinutes(120));

        Location sourceLocation = ride.GetCoordinates(ride.SourceCordinates);

        RideObject rideObject = new(
            ride.Id,
            ride.SourceAddress,
            ride.DestinationAddress,
            [..ride.WaypointAddresses.Split("%%")],
            ride.MusicGenre.ToString(),
            ride.HaveConversation,
            ride.EstimatedFare,
            ride.Payment.Method.ToString()
            );

        // todo: replace...let it come from db...we save it for each ride...
        var res = await _rideService.SendRequestToDriversAsync(
                sourceLocation,
                rideObject,
                rider,
                [],
                request.RideCategory,
                increaseSearchRadius);

        if (!res)
        {
            rider.UpdateStatus(RiderStatus.Online, null);

            // todo: rather than notifying...we do a search again...
            BackgroundJob.Schedule(() => NotifyRiderOfNoDriver(rider.Id), DateTime.UtcNow.AddSeconds(10));
        }
        //else
        //{
        //     we update when driver accepts...
        //    rider.UpdateStatus(RiderStatus.Online, ride.Id);
        //}

        _riderRepository.Update(rider);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // todo: wait time...should rider request via websocket and driver accept via web socket...
        // most likely, we use websockets...
        return new RideRequestResponse(res);
    }

    public async Task NotifyRiderOfNoDriver(long riderId)
    {
        string identifier = RiderKey.CustomNameIdentifier(riderId);

        await _rideHubContext.Clients.User(identifier)
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                Update = ReceiveRideUpdate.NoMatch,
                Data = JsonSerializer.Serialize(new
                {
                    Message = "No match"
                })
            });
    }
}
