using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Rides;
using Soloride.Contracts.Models;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Services;
using Soloride.Domain.Transactions;
using Soloride.Shared.Helper.Keys;

namespace Soloride.Application.Features.Rides.RideRequest;
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

    public RideRequestCommandHandler(IUnitOfWork unitOfWork, IRideService rideService, 
        ICacheService cacheService, IRideRepository rideRepository, IRiderRepository riderRepository,
        IPaymentCardRepository paymentCardRepository, IPaymentRepository paymentRepository,
        IRideLogRepository rideLogRepository)
    {
        _unitOfWork = unitOfWork;
        _rideService = rideService;
        _cacheService = cacheService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _paymentCardRepository = paymentCardRepository;
        _paymentRepository = paymentRepository;
        _rideLogRepository = rideLogRepository;
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

        if (rider.CurrentRideId.HasValue)
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

        // todo: if no record, create new one...
        if (ridePayment is null)
            return Error.NotFound("payment.notfound", "Payment record not found");

        ridePayment.UpdatePaymentMethod(request.PaymentMethod, request.PaymentCardId);

        if (request.RideCategory == RideCategory.Delivery)
            ridePayment.UpdateAmount(ride.EstimatedDeliveryFare);

        _paymentRepository.Update(ridePayment);

        bool increaseSearchRadius = ride.Status == RideStatus.Requested;

        ride.UpdateRideRequest(
            request.RideCategory,
            request.RideConversation, 
            request.MusicGenre);

        _rideRepository.Update(ride);

        RideLog rideLog = new(
            ride.Id,
            RideStatus.Requested);

        await _rideLogRepository.AddAsync(rideLog);

        string rideNotMatchedKey = RideKeys.RideNotMatched(ride.Id);

        await _cacheService.SetAsync(rideNotMatchedKey, 1.ToString(), TimeSpan.FromMinutes(120));

        Location sourceLocation = ride.GetCoordinates(ride.SourceCordinates);

        RideObject rideObject = new(
            ride.Id,
            ride.SourceAddress,
            ride.DestinationAddress,
            ride.MusicGenre.ToString(),
            ride.HaveConversation,
            ride.EstimatedFare,
            ride.Payment.Method.ToString()
            );

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
        }
        else
        {
            rider.UpdateStatus(RiderStatus.Online, ride.Id);
        }

        _riderRepository.Update(rider);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RideRequestResponse(res);
    }
}
