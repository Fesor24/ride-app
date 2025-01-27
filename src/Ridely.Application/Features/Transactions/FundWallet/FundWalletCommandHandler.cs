using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.Payment;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Transactions;

namespace Soloride.Application.Features.Transactions.FundWallet;
internal sealed class FundWalletCommandHandler :
    ICommandHandler<FundWalletCommand, FundWalletResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaystackService _paystackService;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderTransactionHistoryRepository _riderTransactionHistoryRepository;
    private readonly IDriverTransactionHistoryRepository _driverTransactionHistoryRepository;
    private readonly ITransactionReferenceMapRepository _transactionReferenceMapRepository;

    public FundWalletCommandHandler(IUnitOfWork unitOfWork,
        IPaystackService paystackService, IDriverRepository driverRepository,
        IRiderRepository riderRepository, IRiderTransactionHistoryRepository riderTransactionHistoryRepository,
        IDriverTransactionHistoryRepository driverTransactionHistoryRepository, 
        ITransactionReferenceMapRepository transactionReferenceMapRepository)
    {
        _unitOfWork = unitOfWork;
        _paystackService = paystackService;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _riderTransactionHistoryRepository = riderTransactionHistoryRepository;
        _driverTransactionHistoryRepository = driverTransactionHistoryRepository;
        _transactionReferenceMapRepository = transactionReferenceMapRepository;
    }

    public async Task<Result<FundWalletResponse>> Handle(FundWalletCommand request, CancellationToken cancellationToken)
    {
        string email = "";

        Ulid reference = Ulid.NewUlid();

        if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            email = rider.Email;

            RiderTransactionHistory riderTransactionHistory = new(
                rider.Id,
                request.Amount,
                RiderTransactionType.FundWallet,
                reference,
                TransactionStatus.Pending
                );

            TransactionReferenceMap referenceMap = new(reference, TransactionReferenceType.RiderTransaction);

            await _transactionReferenceMapRepository.AddAsync(referenceMap);

            await _riderTransactionHistoryRepository.AddAsync(riderTransactionHistory);
        }
        else if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository
                .GetAsync(request.DriverId.Value);

            if (driver is null) return Error.NotFound("driver.notfound", "Rider not found");

            email = driver.Email;

            DriverTransactionHistory driverTransactionHistory = new(
                driver.Id,
                request.Amount,
                DriverTransactionType.FundWallet,
                reference, TransactionStatus.Pending
                );

            TransactionReferenceMap referenceMap = new(reference, TransactionReferenceType.DriverTransaction);

            await _transactionReferenceMapRepository.AddAsync(referenceMap);

            await _driverTransactionHistoryRepository.AddAsync(driverTransactionHistory);
        }
        else
            return Error.BadRequest("no.user", "Specify rider or driver");

        if (string.IsNullOrWhiteSpace(email))
            return Error.BadRequest("no.email", "Proceed to update your email address then continue");

        var response = await _paystackService.InitializeAsync(new Models.Payment.InitializePayment
        {
            Amount = request.Amount,
            CardVerification = false,
            Email = email,
            Reference = reference.ToString()
        });

        if (response.IsFailure)
        {
            // todo: log error

            return Error.BadRequest("paystack.error", "An error occurred");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FundWalletResponse
        {
            AuthorizationUrl = response.IsSuccessful ? response.Value.Data.AuthorizationUrl : ""
        };
    }
}
