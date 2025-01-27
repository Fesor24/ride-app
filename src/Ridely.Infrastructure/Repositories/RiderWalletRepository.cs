using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Repositories;
internal sealed class RiderWalletRepository(ApplicationDbContext context) :
    GenericRepository<RiderWallet>(context), IRiderWalletRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RiderWallet?> GetByRiderAsync(long riderId) =>
        await _context.Set<RiderWallet>()
            .FirstOrDefaultAsync(wallet => wallet.RiderId == riderId);
}
