using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ridely.Application.Abstractions.Location;
using Ridely.Application.Features.Rides.CancelRideRequest;
using Ridely.Application.Hubs;
using Ridely.Domain.Drivers;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;
using StackExchange.Redis;

namespace Ridely.Infrastructure.Location;
internal sealed class LocationService : ILocationService
{
    private readonly IDatabase _database;
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<RideHub> _rideHubContext;

    public LocationService(IConnectionMultiplexer connectionMultiplexer, 
        ApplicationDbContext context, IHubContext<RideHub> rideHubContext)
    {
        _database = connectionMultiplexer.GetDatabase();
        _context = context;
        _rideHubContext = rideHubContext;
    }

    public async Task UpdateDriverLocationAsync(Domain.Models.Location location, 
        string driverUserKey)
    {
        // add driver to group, then rider, then publish to group
        if (!driverUserKey.StartsWith("DRIVER")) return;

        var driverKeySplit = driverUserKey.Split('-');

        if (driverKeySplit.Length < 2) return;

        string driverId = driverKeySplit[1];

        string driverMatchedKey = RideKeys.Matched(driverId);

        var driverMatched = await _database.StringGetAsync(driverMatchedKey);

        string driverLocationSubscribersKey = DriverKey.LocationSubscribers(long.Parse(driverId));

        var riderSubscribers = await _database.StringGetAsync(driverLocationSubscribersKey);

        // todo: review...this was been hit despite the driver not been on a ride...
        if (riderSubscribers.HasValue)
        {
            string[] riderIdentifiers = riderSubscribers.ToString().Split(".");

            if (riderIdentifiers.Length > 0)
            {
                await _rideHubContext.Clients.Users(riderIdentifiers)
                    .SendAsync(SignalRSubscription.ReceiveNearbyDrivers, new
                    {
                        DriverKey = RideKeys.DriverLocation(driverId),
                        location.Latitude,
                        location.Longitude,
                    });
            }
        }

        string driverLocationUpdateKey = RideKeys.DriverLocation(driverId);

        await _database.GeoAddAsync(ApplicationConstant.REDIS_LOCATIONKEY,
            location.Longitude, location.Latitude, driverLocationUpdateKey);

        if (driverMatched.HasValue)
        {
            await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(long.Parse(driverMatched!)))
               .SendAsync(SignalRSubscription.ReceiveLocationUpdate, new
               {
                   location.Latitude,
                   location.Longitude
               });
        }

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(driverMatched!);

        //var driverLocationMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDER_DRIVERLOCATION_UPDATE,
        //    new
        //    {
        //        latitude = location.Latitude,
        //        longitude = location.Longitude
        //    });

       

        //await _webSocketManager.SendMessageAsync(riderWebSocketKey, driverLocationMessage);
    }

    public async Task DisconnectDriverAsync(long driverId)
    {
        var driver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(x => x.Id == driverId);

        if (driver is null) return;

        string riderLocationSubscribersKey = DriverKey.LocationSubscribers(driverId);

        await _database.KeyDeleteAsync(riderLocationSubscribersKey);

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

    public async Task DeleteLocationRecordAsync(long driverId)
    {
        string sortedSetKey = ApplicationConstant.REDIS_LOCATIONKEY;

        string memberName = RideKeys.DriverLocation(driverId.ToString());

        await _database.SortedSetRemoveAsync(sortedSetKey, memberName);
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

    // should be called every 2 minute...
    public async Task<List<string>> GetNearbyDriversAndStreamDataAsync(Domain.Models.Location location, 
        string riderIdentifier)
    {
        int radius = 1000;

        var result = await _database.GeoSearchAsync(ApplicationConstant.REDIS_LOCATIONKEY,
            location.Longitude, location.Latitude, new GeoSearchCircle(radius, GeoUnit.Meters),
            order: Order.Ascending, options: GeoRadiusOptions.WithDistance);

        // x.Member.ToString sample --> DRIVER.LOCATION-2
        var drivers = result.Select(x => new DriversAvailable(
            x.Member.ToString(),
            x.Distance,
            x.Member.ToString().Split('-').Length > 1 ? x.Member.ToString().Split('-')[1] : "0"
            ));

        long[] availableDriversIds = drivers
            .Select(x => long.Parse(x.DriverId))
            .Take(10)
            .ToArray();

        foreach(var driverId in availableDriversIds)
        {
            var (latitude, longitude, driverLocationKey) = await GetDriverCoordinatesAsync(driverId);

            await _rideHubContext.Clients.User(riderIdentifier)
                .SendAsync(SignalRSubscription.ReceiveNearbyDrivers, new
                {
                    DriverKey = driverLocationKey,
                    Latitude = latitude,
                    Longitude = longitude
                });

            string driverLocationSubscribersKey = DriverKey.LocationSubscribers(driverId);

            // get riders subscribed to a driver
            var riderSubscribers = await _database.StringGetAsync(driverLocationSubscribersKey);

            if (riderSubscribers.HasValue)
            {
                string[] subscribers = riderSubscribers.ToString().Split(".");
                if (!subscribers.Contains(riderIdentifier))
                {
                    string newSubscribers = riderSubscribers.ToString() + "." + riderIdentifier;
                    await _database.StringSetAsync(driverLocationSubscribersKey, newSubscribers);
                }
            }
            else
            {
                await _database.StringSetAsync(driverLocationSubscribersKey, riderIdentifier);
            }
        }
        
        return availableDriversIds.Select(driverId => DriverKey.CustomNameIdentifier(driverId))
            .ToList();
    }

    public async Task<(double Lat, double Long, string LocationKey)> GetDriverCoordinatesAsync(long driverId)
    {
        string driverLocationKey = RideKeys.DriverLocation(driverId.ToString());

        var geoPosition = await _database.GeoPositionAsync(ApplicationConstant.REDIS_LOCATIONKEY, driverLocationKey);

        if (geoPosition.HasValue)
            return (geoPosition.Value.Latitude, geoPosition.Value.Longitude, driverLocationKey);

        return (0, 0, driverLocationKey);
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
