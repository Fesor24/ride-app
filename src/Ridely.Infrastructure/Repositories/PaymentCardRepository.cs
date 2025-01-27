using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Riders;
using Ridely.Domain.Transactions;

namespace Ridely.Infrastructure.Repositories;
internal sealed class PaymentCardRepository(ApplicationDbContext context)
    : GenericRepository<PaymentCard>(context), IPaymentCardRepository
{
    public async Task<IReadOnlyList<PaymentCard>> GetAllByRiderAsync(long riderId) =>
        await context.Set<PaymentCard>()
        .Where(x => x.RiderId == riderId)
        .ToListAsync();
}
