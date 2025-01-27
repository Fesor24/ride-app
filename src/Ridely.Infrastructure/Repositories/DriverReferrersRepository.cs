using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Drivers;

namespace Soloride.Infrastructure.Repositories;
internal sealed class DriverReferrersRepository(ApplicationDbContext context)
    : GenericRepository<DriverReferrers>(context), IDriverReferrersRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<(int Riders, int Drivers)> GetReferredUsersCount(long driverId)
    {
        var referredRiders = await _context.Set<DriverReferrers>()
            .Where(refer => refer.DriverId == driverId)
            .Where(refer => refer.ReferredUser == Domain.Shared.ReferredUser.Rider)
            .CountAsync();

        var referredDrivers = await _context.Set<DriverReferrers>()
            .Where(refer => refer.DriverId == driverId)
            .Where(refer => refer.ReferredUser == Domain.Shared.ReferredUser.Driver)
            .CountAsync();

        return (referredRiders, referredDrivers);
    }
}
