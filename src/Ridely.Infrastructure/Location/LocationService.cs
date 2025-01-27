using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Soloride.Application.Abstractions.Location;
using Soloride.Application.Abstractions.Websocket;
using Soloride.Application.Features.Rides.CancelRideRequest;
using Soloride.Application.Models.WebSocket;
using Soloride.Domain.Drivers;
using Soloride.Shared.Constants;
using Soloride.Shared.Helper;
using Soloride.Shared.Helper.Keys;
using StackExchange.Redis;

namespace Soloride.Infrastructure.Location;
internal sealed class LocationService : ILocationService
{
    private readonly IDatabase _database;
    private readonly IWebSocketManager _webSocketManager;
    private readonly ApplicationDbContext _context;
    public LocationService(IConnectionMultiplexer connectionMultiplexer, 
        IWebSocketManager webSocketManager,
        ApplicationDbContext context)
    {
        _database = connectionMultiplexer.GetDatabase();
        _webSocketManager = webSocketManager;
        _context = context;
    }

    public async Task UpdateDriverLocationAsync(Domain.Models.Location location, string driverUserKey)
    {
        if (!driverUserKey.StartsWith("DRIVER")) return;

        var driverKeySplit = driverUserKey.Split('-');

        if (driverKeySplit.Length < 2) return;

        string driverId = driverKeySplit[1];

        string driverLocationUpdateKey = RideKeys.DriverLocation(driverId);

        await _database.GeoAddAsync(ApplicationConstant.REDIS_LOCATIONKEY,
            location.Longitude, location.Latitude, driverLocationUpdateKey);

        string driverMatchedKey = RideKeys.Matched(driverId);

        var driverMatched = await _database.StringGetAsync(driverMatchedKey);

        if (!driverMatched.HasValue) return;

        string riderWebSocketKey = WebSocketKeys.Rider.Key(driverMatched!);

        var driverLocation = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.RIDER_DRIVERLOCATION_UPDATE,
            Payload = new
            {
                latitude = location.Latitude,
                longitude = location.Longitude
            }
        };

        string driverLocationMessage = Serialize.Object(driverLocation);

        await _webSocketManager.SendMessageAsync(riderWebSocketKey, driverLocationMessage);
    }

    public async Task DisconnectDriverAsync(long driverId)
    {
        var driver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(x => x.Id == driverId);

        if (driver is null) return;

        string sortedSetKey = ApplicationConstant.REDIS_LOCATIONKEY;

        string memberName = RideKeys.DriverLocation(driverId.ToString());

        var location = await _database.GeoPositionAsync(sortedSetKey, memberName);

        if (location.HasValue && driver.CurrentRideId.HasValue)
        {
            driver.SetStatusAndUpdateLocation(location.Value.Latitude, location.Value.Longitude, driver.Status);
        }
        else if(location.HasValue && !driver.CurrentRideId.HasValue)
        {
            driver.SetStatusAndUpdateLocation(location.Value.Latitude, location.Value.Longitude, DriverStatus.Offline);

            await _database.SortedSetRemoveAsync(sortedSetKey, memberName);
        }
        else
        {
            driver.SetStatusAndUpdateLocation(0, 0, DriverStatus.Offline);

            await _database.SortedSetRemoveAsync(sortedSetKey, memberName);
        }

        _context.Set<Driver>().Update(driver);

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetAvailableDriversCountInLocationAsync(Domain.Models.Location location, 
        List<long> excludeDrivers, long riderId, CabType? cabType, DriverService? driverService)
    {
        int radius = 3000;

        var result = await _database.GeoSearchAsync(ApplicationConstant.REDIS_LOCATIONKEY,
            location.Longitude, location.Latitude, new GeoSearchCircle(radius, GeoUnit.Meters),
            order: Order.Ascending, options: GeoRadiusOptions.WithDistance);

        // x.Member.ToString sample --> DRIVER.LOCATION-2
        var drivers = result.Select(x => new DriversAvailable(
            x.Member.ToString(),
            x.Distance,
            x.Member.ToString().Split('-').Length > 1 ? x.Member.ToString().Split('-')[1] : "0"
            ));

        excludeDrivers.AddRange(await GetCancelledDrivers(riderId.ToString()));

        long[] availableDriversIds = drivers
            .Select(x => long.Parse(x.DriverId))
            .Where(x => !excludeDrivers.Contains(x))
            .ToArray();

        return await _context.Set<Driver>()
            .Include(driver => driver.Cab)
            .Where(driver => availableDriversIds.Contains(driver.Id))
            .Where(driver => driver.Status == DriverStatus.Online)
            .Where(driver => driver.IdentityValidated)
            .Where(driver => cabType.HasValue && driver.Cab.CabType == cabType.Value)
            .Where(driver => driverService.HasValue && driver.DriverService == driverService.Value)
            .CountAsync();
    }

    public async Task<(double Lat, double Long)> GetDriverCoordinatesAsync(string driverLocationKey)
    {
        var geoPosition = await _database.GeoPositionAsync(ApplicationConstant.REDIS_LOCATIONKEY, driverLocationKey);

        if (geoPosition.HasValue)
            return (geoPosition.Value.Latitude, geoPosition.Value.Longitude);

        return (0, 0);
    }

    private async Task<List<long>> GetCancelledDrivers(string riderId)
    {
        string key = RideKeys.DriversCancelled(riderId);

        var cancelledDrivers = await _database.StringGetAsync(key);

        if (cancelledDrivers.IsNullOrEmpty) return [];

        var drivers = JsonSerializer.Deserialize<List<CancelledDrivers>>(cancelledDrivers!) ?? [];

        List<long> driverIds = [];

        driverIds.AddRange(drivers
            .Where(driver => driver.Expiry >= DateTime.UtcNow)
            .Select(driver => driver.DriverId)
            .ToList());

        return driverIds;
    }
}
