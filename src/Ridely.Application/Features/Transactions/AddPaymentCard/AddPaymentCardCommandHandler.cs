using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Transactions.AddPaymentCard;
internal sealed class AddPaymentCardCommandHandler :
    ICommandHandler<AddPaymentCardCommand, AddPaymentCardResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaystackService _paystackService;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderTransactionHistoryRepository _riderTransactionHistoryRepository;
    private readonly ITransactionReferenceMapRepository _transactionReferenceMapRepository;

    public AddPaymentCardCommandHandler(IUnitOfWork unitOfWork, IPaystackService paystackService,
        IRiderRepository riderRepository, IRiderTransactionHistoryRepository riderTransactionHistoryRepository,
        ITransactionReferenceMapRepository transactionReferenceMapRepository)
    {
        _unitOfWork = unitOfWork;
        _paystackService = paystackService;
        _riderRepository = riderRepository;
        _riderTransactionHistoryRepository = riderTransactionHistoryRepository;
        _transactionReferenceMapRepository = transactionReferenceMapRepository;
    }

    public async Task<Result<AddPaymentCardResponse>> Handle(AddPaymentCardCommand request,
        CancellationToken cancellationToken)
    {
        var rider = await _riderRepository
            .GetDetailsAsync(request.RiderId);

        if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

        Ulid reference = Ulid.NewUlid();

        RiderTransactionHistory riderTransactionHistory = new(
            rider.Id,
            50,
            RiderTransactionType.CardAddition,
            reference,
            TransactionStatus.Pending,
            Domain.Rides.PaymentProvider.Paystack
            );

        await _riderTransactionHistoryRepository.AddAsync(riderTransactionHistory);

        TransactionReferenceMap transactionReferenceMap = new(reference,
            TransactionReferenceType.RiderTransaction);

        await _transactionReferenceMapRepository.AddAsync(transactionReferenceMap);

        var initializeResponse = await _paystackService.InitializeAsync(new Models.Payment.InitializePayment
        {
            Email = rider.Email,
            CardVerification = true,
            Reference = reference.ToString()
        }, true);

        if (initializeResponse.IsFailure)
        {
            //todo: log error
            return Error.BadRequest("paystack.error", "An error occurred");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddPaymentCardResponse
        {
            AuthorizationUrl = initializeResponse.Value.Data.AuthorizationUrl
        };
    }
}
