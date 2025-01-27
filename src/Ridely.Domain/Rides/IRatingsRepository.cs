using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Rides;
public interface IRatingsRepository : IGenericRepository<Ratings>
{
    Task<Ratings?> GetByRideAsync(long rideId);
}
