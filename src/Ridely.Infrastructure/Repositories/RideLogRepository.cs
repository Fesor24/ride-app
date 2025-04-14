using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Repositories;
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

    public async Task<IReadOnlyList<RideLog>> GetLogsByStatuses(IEnumerable<RideLogEvent> rideEvents)
    {
        return await _context.Set<RideLog>()
            .Where(log => rideEvents.Contains(log.Event))
            .AsNoTracking()
            .ToListAsync();
    }
}
