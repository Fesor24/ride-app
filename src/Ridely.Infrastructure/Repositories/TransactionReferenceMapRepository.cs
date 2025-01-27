using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Transactions;

namespace Soloride.Infrastructure.Repositories;
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
