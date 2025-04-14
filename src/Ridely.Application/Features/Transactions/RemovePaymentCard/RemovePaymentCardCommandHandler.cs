using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Transactions.RemovePaymentCard;
internal sealed class RemovePaymentCardCommandHandler :
    ICommandHandler<RemovePaymentCardCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideRepository _rideRepository;
    private readonly IPaymentCardRepository _paymentCardRepository;

    public RemovePaymentCardCommandHandler(IUnitOfWork unitOfWork, IRideRepository rideRepository,
        IPaymentCardRepository paymentCardRepository)
    {
        _unitOfWork = unitOfWork;
        _rideRepository = rideRepository;
        _paymentCardRepository = paymentCardRepository;
    }

    public async Task<Result<bool>> Handle(RemovePaymentCardCommand request, CancellationToken cancellationToken)
    {
        var paymentCard = await _paymentCardRepository
            .GetAsync(request.PaymentCardId);

        if (paymentCard is null) return Error.NotFound("paymentcard.notfound",
            "Payment card not found");

        var activeRides = await _rideRepository
            .Search(new Domain.Models.Rides.RideSearchParams
            {
                RiderId = request.RiderId,
                PageNumber = 1,
                PageSize = 10,
                RideStatus = [RideStatus.Matched, RideStatus.Started]
            });

        if (activeRides.TotalItems > 0) return Error.BadRequest("disallowed",
            "You can not remove payment cards when a ride has been matched or in progress");

        _paymentCardRepository.Delete(paymentCard);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
