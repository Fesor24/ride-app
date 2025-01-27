using Ridely.Domain.Drivers;

namespace Ridely.Application.Abstractions.Location;
public interface ILocationService
{
    Task UpdateDriverLocationAsync(Domain.Models.Location location, string driverUserKey);
    Task DisconnectDriverAsync(long driverId);
    Task<int> GetAvailableDriversCountInLocationAsync(Domain.Models.Location location,
         List<long> excludeDrivers, long riderId, CabType? cabType, DriverService? driverService);

    Task<(double Lat, double Long)> GetDriverCoordinatesAsync(string driverLocationKey);
}
