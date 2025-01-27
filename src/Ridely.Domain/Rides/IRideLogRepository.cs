using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Rides;
public interface IRideLogRepository : IGenericRepository<RideLog>
{
    Task<IReadOnlyList<RideLog>> GetLogsByRide(long rideId);
}
