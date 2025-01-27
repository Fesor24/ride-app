using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Rides;

namespace Soloride.Domain.Rides;
public interface IRideRepository : IGenericRepository<Ride>
{
    Task<Ride?> GetRideDetails(long rideId);
    Task<PaginatedList<RideModel>> Search(RideSearchParams searchParams);
    Task<Ride?> GetLatestDriverRideDetails(long driverId);
    Task<int> GetTotalCountAsync(RideStatus status);
}
