using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Repositories;
internal sealed class PaymentDetailRepository(ApplicationDbContext context) : GenericRepository<PaymentDetail>(context),
    IPaymentDetailRepository
{
    private readonly DbSet<PaymentDetail> _dbSet = context.Set<PaymentDetail>();

    public async Task<List<PaymentDetail>> GetAllByPaymentId(long paymentId) =>
        await _dbSet
            .Where(detail => detail.PaymentId == paymentId)
            .ToListAsync();

    public async Task<IEnumerable<PaymentDetail>> GetAllByReference(List<Ulid> paymentReferences) =>
        await _dbSet
        .Where(detail => paymentReferences.Contains(detail.Reference))
        .ToListAsync();

    public async Task<PaymentDetail?> GetAsync(PaymentFor paymentFor, long paymentId) =>
        await _dbSet
            .Where(detail => detail.PaymentId == paymentId && detail.PaymentFor == paymentFor)
            .FirstOrDefaultAsync();

    public async Task<PaymentDetail?> GetByReferenceAsync(Ulid reference) =>
        await _dbSet
            .FirstOrDefaultAsync(detail => detail.Reference == reference);
}
