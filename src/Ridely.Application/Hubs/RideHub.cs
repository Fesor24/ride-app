using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Location;
using Ridely.Application.Abstractions.Rides;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Hubs;
public class RideHub : Hub
{
    private readonly ILocationService _locationService;
    private readonly IRideService _rideService;
    private readonly ICacheService _cacheService;

    public RideHub(ILocationService locationService, IRideService rideService, ICacheService cacheService)
    {
        _locationService = locationService;
        _rideService = rideService;
        _cacheService = cacheService;
    }

    public async Task UpdateLocation(DriverLocationUpdate location)
    {
        var role = Context?.User?.FindFirstValue(ClaimTypes.Role);

        if (role != Roles.Driver) return;

        var driverId = Context?.User?.FindFirstValue(ClaimsConstant.Driver) ?? "0";

        if (long.TryParse(driverId, out long driverIdentifier) && driverIdentifier != 0)
        {
            await _locationService.UpdateDriverLocationAsync(new Domain.Models.Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Sequence = location.Sequence
            }, DriverKey.LocationUpdates(driverIdentifier));
        }

        Console.WriteLine($"Driver with Id: {driverId} location updated");
    }

    // should be called every 3-5 minutes...
    // this would stream nearby drivers location to rider and update subscribers to the driver...
    public async Task GetNearbyDrivers(RiderLocationUpdate location)
    {
        string nameIdentifier = Context?.UserIdentifier ?? "";

        if (!nameIdentifier.StartsWith("rider", StringComparison.InvariantCultureIgnoreCase))
            return;

        await _locationService.GetNearbyDriversAndStreamDataAsync(new Domain.Models.Location
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude,
        }, nameIdentifier);
    }

    public async Task Chat(RideChat chat)
    {
        UserType sender = UserType.Driver;

        string identifier = Context?.UserIdentifier ?? "";

        if (identifier.StartsWith("driver", StringComparison.InvariantCultureIgnoreCase))
            sender = UserType.Driver;

        else if (identifier.StartsWith("rider", StringComparison.InvariantCultureIgnoreCase))
            sender = UserType.Rider;

        else return;

        await _rideService.SendChatMessageAsync(sender, chat.Message, identifier, chat.RideId);
    }

    public override Task OnConnectedAsync()
    {
        string identifier = Context?.UserIdentifier ?? "";
        Console.WriteLine($"User with Id: {identifier} connected");

        if (identifier.StartsWith("rider", StringComparison.InvariantCultureIgnoreCase))
        {
            string[] split = identifier.Split("-");

            if (split.Length != 2) return Task.CompletedTask;

            if (long.TryParse(split[1], out long riderId))
            {
                return _cacheService.RemoveAsync(RiderKey.Disconnected(riderId));
            }
        }
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        string identifier = Context?.UserIdentifier ?? "";

        if (identifier.StartsWith("driver", StringComparison.InvariantCultureIgnoreCase))
        {
            string[] split = identifier.Split("-");

            if (split.Length != 2) return Task.CompletedTask;

            if (long.TryParse(split[1], out long driverId))
            {
                return _locationService.DisconnectDriverAsync(driverId);
            }
        }

        if (identifier.StartsWith("rider", StringComparison.InvariantCultureIgnoreCase))
        {
            string[] split = identifier.Split("-");

            if (split.Length != 2) return Task.CompletedTask;

            if (long.TryParse(split[1], out long riderId))
            {
                return _cacheService.SetAsync(RiderKey.Disconnected(riderId), 1.ToString(), TimeSpan.FromMinutes(10));
            }
        }

        return base.OnDisconnectedAsync(exception);
    }
}

public sealed record DriverLocationUpdate(
    double Latitude, double Longitude, int Sequence);

public sealed record RiderLocationUpdate(
    double Latitude, double Longitude);

public sealed record RideChat(string Message, long RideId);