using Soloride.Domain.Abstractions;
using Soloride.Domain.Models.Drivers;
using Soloride.Domain.Models;

namespace Soloride.Domain.Drivers;
public interface IDriverTransactionHistoryRepository : IGenericRepository<DriverTransactionHistory>
{
    Task<DriverTransactionHistory?> GetByReferenceAsync(Ulid reference);
    Task<PaginatedList<DriverTransactionModel>> Search(DriverTransactionSearchParams searchParams);
}
