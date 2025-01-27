using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Drivers.VerifyBankAccount;
internal sealed class VerifyBankAccountQueryHandler :
    IQueryHandler<VerifyBankAccountQuery, VerifyBankAccountResponse>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IBankRepository _bankRepository;
    private readonly IPaystackService _paystackService;

    public VerifyBankAccountQueryHandler(IDriverRepository driverRepository, IBankRepository bankRepository,
        IPaystackService paystackService)
    {
        _driverRepository = driverRepository;
        _bankRepository = bankRepository;
        _paystackService = paystackService;
    }

    public async Task<Result<VerifyBankAccountResponse>> Handle(VerifyBankAccountQuery request,
        CancellationToken cancellationToken)
    {
        var driver = await _driverRepository
           .GetAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        var bank = await _bankRepository
            .GetAsync(request.BankId);

        if (bank is null) return Error.NotFound("bank.notfound", "Bank not found");

        var res = await _paystackService
            .ResolveAccount(bank.Code, request.AccountNo);

        if (res.IsFailure)
            return res.Error;

        return new VerifyBankAccountResponse
        {
            AccountName = res.Value.Data.AccountName,
            AccountNo = res.Value.Data.AccountNumber
        };
    }
}
