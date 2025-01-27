using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Rides;

namespace Soloride.Infrastructure.Repositories;
internal sealed class PaymentRepository(ApplicationDbContext context)
    : GenericRepository<Payment>(context), IPaymentRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Payment?> GetByReferenceAsync(Ulid reference)
    {
        return await _context.Set<Payment>()
            .FirstOrDefaultAsync(payment => payment.Reference == reference);
    }
}
