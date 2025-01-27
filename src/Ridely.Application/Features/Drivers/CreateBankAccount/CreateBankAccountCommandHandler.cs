using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;
using Ridely.Domain.Services;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Drivers.CreateBankAccount;
internal sealed class CreateBankAccountCommandHandler :
    ICommandHandler<CreateBankAccountCommand>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IPaystackService _paystackService;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IBankRepository _bankRepository;

    public CreateBankAccountCommandHandler(IDriverRepository driverRepository, IUnitOfWork unitOfWork,
        ICacheService cacheService, IPaystackService paystackService, IBankAccountRepository bankAccountRepository,
        IBankRepository bankRepository)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _paystackService = paystackService;
        _bankAccountRepository = bankAccountRepository;
        _bankRepository = bankRepository;
    }

    public async Task<Result<bool>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository
            .GetAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        string key = Cache.BankAccount.Key(driver.PhoneNo!);

        string? code = await _cacheService.GetAsync(key);

        if (string.IsNullOrWhiteSpace(code) || code != request.Otp)
            return Error.BadRequest("invalid.otp", "Invalid otp");

        var driverBankAccounts = await _bankAccountRepository
            .GetAllByDriverAsync(driver.Id);

        if (driverBankAccounts.Any(x => x.AccountNo == request.AccountNo))
            return Error.BadRequest("account.duplicate", "Account number exists");

        if (driverBankAccounts.Count == 3)
            return Error.BadRequest("maximum.bankaccounts",
                "Maximum bank account that can be added is 3");

        var bank = await _bankRepository
            .GetAsync(request.BankId);

        if (bank is null) return Error.NotFound("bank.notfound", "Bank not found");

        var resolveRes = await _paystackService.ResolveAccount(bank.Code, request.AccountNo);

        if (resolveRes.IsFailure)
            return resolveRes.Error;

        var res = await _paystackService.CreateRecipient(resolveRes.Value.Data.AccountName, bank.Code,
            resolveRes.Value.Data.AccountNumber, bank.Type);

        string recipientCode = "";
        if (res.IsSuccessful) recipientCode = res.Value.Data.RecipientCode;

        else return res.Error;

        BankAccount driverBankAccount = new(
            driver.Id,
            resolveRes.Value.Data.AccountNumber,
            resolveRes.Value.Data.AccountName,
            bank.Id,
            recipientCode
            );

        await _bankAccountRepository.AddAsync(driverBankAccount);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
