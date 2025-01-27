using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Rides;

namespace Soloride.Infrastructure.Repositories;
internal sealed class RideLogRepository(ApplicationDbContext context) :
    GenericRepository<RideLog>(context), IRideLogRepository
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IReadOnlyList<RideLog>> GetLogsByRide(long rideId)
    {
        return await _context.Set<RideLog>()
            .Where(log => log.RideId == rideId)
            .ToListAsync();
    }
}
