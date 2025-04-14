using FirebaseAdmin.Auth;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Rides;
using Ridely.Infrastructure.WebSockets.Attributes;

namespace Ridely.Infrastructure.WebSockets.Handlers;

//[WebSocketClass("RIDE")]
//internal sealed class RideWebSocketEventHandler : IWebSocketEventHandlerMarker
//{
//    private readonly IRideService _rideService;

//    public RideWebSocketEventHandler(IRideService rideService)
//    {
//        _rideService = rideService;
//    }

//    [WebSocketMethod("HELLO")]
//    public async Task<object> Hello(string name, string userIdentifier)
//    {
//        var response = new WebSocketResponse<string>()
//        {
//            Event = "Greetings",
//            Payload = "Name is " + name + "with Id: " + userIdentifier
//        };

//        return await Task.FromResult(response);

//    }

//    [WebSocketMethod("CHAT")]
//    public async Task Chat(string message, int rideId, bool isDriver)
//    {
//        await _rideService.SendChatMessageAsync(message, rideId, isDriver);
//    }
//}
