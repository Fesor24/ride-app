using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class DriverDiscountRepository(ApplicationDbContext context) : GenericRepository<DriverDiscount>(context),
    IDriverDiscountRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<DriverDiscount?> GetDiscountByDriverAndTypeAsync(long driverId, DriverDiscountType type) =>
        await _context.Set<DriverDiscount>()
            .Where(disc => disc.DriverId == driverId && disc.Type == type)
            .FirstOrDefaultAsync();
}
