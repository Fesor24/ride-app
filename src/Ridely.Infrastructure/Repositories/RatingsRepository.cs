using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Repositories;
internal sealed class RatingsRepository(ApplicationDbContext context) :
    GenericRepository<Ratings>(context), IRatingsRepository
{
    public async Task<Ratings?> GetByRideAsync(long rideId) =>
        await context.Set<Ratings>()
        .FirstOrDefaultAsync(rating => rating.RideId == rideId);
}
