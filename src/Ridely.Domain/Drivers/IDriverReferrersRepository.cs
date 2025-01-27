using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Drivers;
public interface IDriverReferrersRepository : IGenericRepository<DriverReferrers>
{
    Task<(int Riders, int Drivers)> GetReferredUsersCount(long driverId);
}
