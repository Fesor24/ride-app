using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Drivers.GetBankAcounts;
internal sealed class GetBankAccountsQueryHandler(IBankAccountRepository bankAccountRepository) :
    IQueryHandler<GetBankAccountsQuery, List<GetBankAccountsResponse>>
{
    public async Task<Result<List<GetBankAccountsResponse>>> Handle(GetBankAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var driverBankAccounts = await bankAccountRepository
            .GetAllByDriverAsync(request.DriverId);

        if (driverBankAccounts is null || driverBankAccounts.Count == 0)
            return new List<GetBankAccountsResponse>();

        return driverBankAccounts.ConvertAll(x => new GetBankAccountsResponse
        {
            Id = x.Id,
            AccountName = x.AccountName,
            BankName = x.Bank.Name,
            AccountNo = x.AccountNo
        });
    }
}
