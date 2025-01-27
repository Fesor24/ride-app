using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Riders;
public interface IRiderReferrersRepository : IGenericRepository<RiderReferrers>
{
    Task<(int Riders, int Drivers)> GetReferredUsersCount(long riderId);
}
