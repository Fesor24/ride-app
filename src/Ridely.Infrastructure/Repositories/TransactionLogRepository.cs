using Ridely.Domain.Transactions;

namespace Ridely.Infrastructure.Repositories;
internal sealed class TransactionLogRepository(ApplicationDbContext context) : 
    GenericRepository<TransactionLog>(context), ITransactionLogRepository
{
}
