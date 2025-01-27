using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;

namespace Ridely.Domain.Rides;
public interface IRideRepository : IGenericRepository<Ride>
{
    Task<Ride?> GetRideDetails(long rideId);
    Task<PaginatedList<RideModel>> Search(RideSearchParams searchParams);
    Task<Ride?> GetLatestDriverRideDetails(long driverId);
    Task<int> GetTotalCountAsync(RideStatus status);
}
