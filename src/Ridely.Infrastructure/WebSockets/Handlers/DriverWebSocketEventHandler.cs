using Ridely.Application.Abstractions.Location;
using Ridely.Domain.Models;
using Ridely.Infrastructure.WebSockets.Attributes;

namespace Ridely.Infrastructure.WebSockets.Handlers;

[WebSocketClass("DRIVER")]
internal sealed class DriverWebSocketEventHandler : IWebSocketEventHandlerMarker
{
    private readonly ILocationService _locationService;

    public DriverWebSocketEventHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [WebSocketMethod("LOCATION")]
    public async Task UpdateLocationAsync(double latitude, double longitude, string userId)
    {
        Console.WriteLine("Location updated");

        await _locationService.UpdateDriverLocationAsync(new Location
        {
            Latitude = latitude,
            Longitude = longitude
        }, userId);
    }
}
