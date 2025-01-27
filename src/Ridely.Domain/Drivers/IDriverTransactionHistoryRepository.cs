using Ridely.Domain.Abstractions;
using Ridely.Domain.Models.Drivers;
using Ridely.Domain.Models;

namespace Ridely.Domain.Drivers;
public interface IDriverTransactionHistoryRepository : IGenericRepository<DriverTransactionHistory>
{
    Task<DriverTransactionHistory?> GetByReferenceAsync(Ulid reference);
    Task<PaginatedList<DriverTransactionModel>> Search(DriverTransactionSearchParams searchParams);
}
