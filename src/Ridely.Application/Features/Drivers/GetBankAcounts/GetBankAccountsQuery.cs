using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.GetBankAcounts;
public sealed record GetBankAccountsQuery(long DriverId) :
    IQuery<List<GetBankAccountsResponse>>;
