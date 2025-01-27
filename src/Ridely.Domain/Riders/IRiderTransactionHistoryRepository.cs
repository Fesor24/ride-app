using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Riders;

namespace Ridely.Domain.Riders;
public interface IRiderTransactionHistoryRepository : IGenericRepository<RiderTransactionHistory>
{
    Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference);
    Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference, RiderTransactionType transactionType);
    Task<PaginatedList<RiderTransactionModel>> Search(RiderTransactionSearchParams searchParams);
}
