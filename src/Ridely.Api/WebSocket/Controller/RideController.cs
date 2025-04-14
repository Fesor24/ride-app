using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Domain.Rides;
using RidelyAPI.WebSocket.Attributes;

namespace RidelyAPI.WebSocket.Controller;

//[WebSocketRoute("RIDE")]
//public class RideController(IRideService rideService) : WebSocketControllerBase
//{
//    // UserIdentifier sample is DRIVER-2
//    [WebSocketEventName("HELLO")]
//    public async Task<object> Hello(string name)
//    {
//        var response = WebSocketMessage<object>.Create(
//            "Greetings",
//            new
//            {
//                message = "Name is " + name + "with Id: " + UserIdentifier
//            });

//        return await Task.FromResult(response);
//    }

//    [WebSocketEventName("CHAT")]
//    public async Task Chat(string message, int rideId)
//    {
//        string[] splitId = UserIdentifier.Split('-');

//        if (UserIdentifier.StartsWith("DRIVER"))
//        {
//            await rideService.SendChatMessageAsync(ChatUserType.Driver, message, splitId[1], rideId);
//        }

//        else if (UserIdentifier.StartsWith("RIDER"))
//        {
//            await rideService.SendChatMessageAsync(ChatUserType.Rider, message, splitId[1], rideId);
//        }
//    }
//}
