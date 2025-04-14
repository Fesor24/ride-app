using Hangfire;
using Ridely.Application.Abstractions.Location;
using Ridely.Domain.Models;
using RidelyAPI.WebSocket.Attributes;

namespace RidelyAPI.WebSocket.Controller;

//[WebSocketRoute("DRIVER")]
//public class DriverController(ILocationService locationService) : WebSocketControllerBase
//{
//    [WebSocketEventName("LOCATION")]
//    public async Task Location(double latitude, double longitude)
//    {
//        Console.WriteLine("Location updated");

//        await locationService.UpdateDriverLocationAsync(new Location 
//        { 
//            Latitude = latitude,
//            Longitude = longitude
//        }, UserIdentifier);
//    }

//    [WebSocketEventName("DISCONNECT")]
//    public override async void Disconnect(string userIdentifier)
//    {
//        if (userIdentifier.StartsWith("RIDER")) return;

//        string[] splitIdentifier = userIdentifier.Split("-");

//        if (splitIdentifier.Length < 2) return;

//        string driverIdentifier = splitIdentifier[1];

//        bool idParsed = int.TryParse(driverIdentifier, out int driverId);

//        if (!idParsed) return;

//        Console.WriteLine("Driver with Id: " + driverId + " disconnected");

//        BackgroundJob.Enqueue(() => locationService.DisconnectDriverAsync(driverId));

//        base.Disconnect(userIdentifier);
//    }
//}
