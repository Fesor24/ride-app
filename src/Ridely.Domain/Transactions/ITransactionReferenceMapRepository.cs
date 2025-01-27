using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Transactions;
public interface ITransactionReferenceMapRepository : IGenericRepository<TransactionReferenceMap>
{
    Task<TransactionReferenceMap?> GetByReferenceAsync(Ulid reference);
}
