using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public interface IRideLogRepository : IGenericRepository<RideLog>
{
    Task<IReadOnlyList<RideLog>> GetLogsByRide(long rideId);
    Task<IReadOnlyList<RideLog>> GetLogsByStatuses(IEnumerable<RideLogEvent> rideLogEvents);
}
