using System.Text.Json;
using Microsoft.Azure.WebPubSub.AspNetCore;
using Microsoft.Azure.WebPubSub.Common;

namespace Ridely.Infrastructure.WebSockets.Hub;
public sealed class MainApplicationHub : WebPubSubHub
{
    private readonly WebPubSubServiceClient<MainApplicationHub> _webPubSubServiceClient;
    private readonly WebSocketEventHandler _webSocketEventHandler;

    public MainApplicationHub(WebPubSubServiceClient<MainApplicationHub> webPubSubServiceClient,
        WebSocketEventHandler webSocketEventHandler)
    {
        _webPubSubServiceClient = webPubSubServiceClient;
        _webSocketEventHandler = webSocketEventHandler;
    }

    public override async Task OnConnectedAsync(ConnectedEventRequest request)
    {
        Console.WriteLine($"User with Id: {request.ConnectionContext.UserId} connected");

        await Task.CompletedTask;
    }

    public override async ValueTask<UserEventResponse> OnMessageReceivedAsync(UserEventRequest request, 
        CancellationToken cancellationToken)
    {
        try
        {
            var webSocketEvent = JsonSerializer.Deserialize<WebSocketEvent>(request.Data.ToString());

            if (webSocketEvent is null) return new UserEventResponse();

            Console.WriteLine(webSocketEvent.EventName);

            webSocketEvent.EventArgs.Add("userId", request.ConnectionContext.UserId);

            await _webSocketEventHandler.DispatchAsync(webSocketEvent);

            await Task.CompletedTask;

            return new UserEventResponse();
        }
        catch(Exception)
        {
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(DisconnectedEventRequest request)
    {
        Console.WriteLine($"User with Id: {request.ConnectionContext.UserId} disconnected");

        await Task.CompletedTask;
    }
}