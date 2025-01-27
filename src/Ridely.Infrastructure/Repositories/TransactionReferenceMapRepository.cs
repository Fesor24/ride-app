using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Transactions;

namespace Ridely.Infrastructure.Repositories;
internal sealed class TransactionReferenceMapRepository(ApplicationDbContext context) :
    GenericRepository<TransactionReferenceMap>(context), ITransactionReferenceMapRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<TransactionReferenceMap?> GetByReferenceAsync(Ulid reference)
    {
        return await _context.Set<TransactionReferenceMap>()
            .FirstOrDefaultAsync(transaction => transaction.Reference == reference);
    }
}
