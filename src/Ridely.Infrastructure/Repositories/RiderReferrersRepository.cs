using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Riders;

namespace Soloride.Infrastructure.Repositories;
internal sealed class RiderReferrersRepository(ApplicationDbContext context) : 
    GenericRepository<RiderReferrers>(context), IRiderReferrersRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<(int Riders, int Drivers)> GetReferredUsersCount(long riderId)
    {
        var referredRiders = await _context.Set<RiderReferrers>()
            .Where(refer => refer.RiderId == riderId)
            .Where(refer => refer.ReferredUser == Domain.Shared.ReferredUser.Rider)
            .CountAsync();

        var referredDrivers = await _context.Set<RiderReferrers>()
            .Where(refer => refer.RiderId == riderId)
            .Where(refer => refer.ReferredUser == Domain.Shared.ReferredUser.Driver)
            .CountAsync();

        return (referredRiders, referredDrivers);
    }
}
