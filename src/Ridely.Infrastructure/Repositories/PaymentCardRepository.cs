using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Riders;
using Soloride.Domain.Transactions;

namespace Soloride.Infrastructure.Repositories;
internal sealed class PaymentCardRepository(ApplicationDbContext context)
    : GenericRepository<PaymentCard>(context), IPaymentCardRepository
{
    public async Task<IReadOnlyList<PaymentCard>> GetAllByRiderAsync(long riderId) =>
        await context.Set<PaymentCard>()
        .Where(x => x.RiderId == riderId)
        .ToListAsync();
}
