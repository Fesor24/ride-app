using Soloride.Domain.Transactions;

namespace Soloride.Infrastructure.Repositories;
internal sealed class TransactionLogRepository(ApplicationDbContext context) : 
    GenericRepository<TransactionLog>(context), ITransactionLogRepository
{
}
