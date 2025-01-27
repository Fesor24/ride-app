using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public interface IDriverReferrersRepository : IGenericRepository<DriverReferrers>
{
    Task<(int Riders, int Drivers)> GetReferredUsersCount(long driverId);
}
