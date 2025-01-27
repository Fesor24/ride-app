using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Riders;

namespace Soloride.Domain.Riders;
public interface IRiderTransactionHistoryRepository : IGenericRepository<RiderTransactionHistory>
{
    Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference);
    Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference, RiderTransactionType transactionType);
    Task<PaginatedList<RiderTransactionModel>> Search(RiderTransactionSearchParams searchParams);
}
