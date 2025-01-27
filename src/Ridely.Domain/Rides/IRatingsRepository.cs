using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public interface IRatingsRepository : IGenericRepository<Ratings>
{
    Task<Ratings?> GetByRideAsync(long rideId);
}
