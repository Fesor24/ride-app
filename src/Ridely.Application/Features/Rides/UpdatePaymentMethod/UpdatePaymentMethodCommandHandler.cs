using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Transactions;

namespace Soloride.Application.Features.Rides.UpdatePaymentMethod;
internal sealed class UpdatePaymentMethodCommandHandler : ICommandHandler<UpdatePaymentMethodCommand>
{
    private readonly IRideRepository _rideRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentCardRepository _paymentCardRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentMethodCommandHandler(IRideRepository rideRepository,
        IPaymentRepository paymentRepository, IPaymentCardRepository paymentCardRepository,
        IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _paymentRepository = paymentRepository;
        _paymentCardRepository = paymentCardRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdatePaymentMethodCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        List<RideStatus> statuses = [RideStatus.Arrived, RideStatus.Matched];

        if (!statuses.Contains(ride.Status))
            return Error.BadRequest("invalid.ridestatus", "Payment option can only be switched before a ride start");

        if (request.PaymentMethod == PaymentMethod.Card)
        {
            if (!request.PaymentCardId.HasValue) return Error.BadRequest("payment.card", "Select a payment card");

            var paymentCards = await _paymentCardRepository.GetAllByRiderAsync(ride.RiderId);

            if (!paymentCards.Any(card => card.Id == request.PaymentCardId.Value))
                return Error.NotFound("paymentcard.notfound", "Payment card not found");

            ride.Payment.UpdatePaymentMethod(PaymentMethod.Card, request.PaymentCardId.Value);
        }
        else
            ride.Payment.UpdatePaymentMethod(PaymentMethod.Cash);

        _paymentRepository.Update(ride.Payment);

        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}
