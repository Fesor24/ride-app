using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.GetBankAcounts;
public sealed record GetBankAccountsQuery(long DriverId) :
    IQuery<List<GetBankAccountsResponse>>;
