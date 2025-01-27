using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Rides;

namespace Soloride.Infrastructure.Repositories;
internal sealed class RatingsRepository(ApplicationDbContext context) :
    GenericRepository<Ratings>(context), IRatingsRepository
{
    public async Task<Ratings?> GetByRideAsync(long rideId) =>
        await context.Set<Ratings>()
        .FirstOrDefaultAsync(rating => rating.RideId == rideId);
}
