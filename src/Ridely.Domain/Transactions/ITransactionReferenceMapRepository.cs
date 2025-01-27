using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Transactions;
public interface ITransactionReferenceMapRepository : IGenericRepository<TransactionReferenceMap>
{
    Task<TransactionReferenceMap?> GetByReferenceAsync(Ulid reference);
}
