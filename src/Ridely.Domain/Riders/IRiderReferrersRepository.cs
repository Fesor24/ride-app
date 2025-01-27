using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Riders;
public interface IRiderReferrersRepository : IGenericRepository<RiderReferrers>
{
    Task<(int Riders, int Drivers)> GetReferredUsersCount(long riderId);
}
