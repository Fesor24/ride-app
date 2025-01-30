using Azure.Core;
using Microsoft.Azure.WebPubSub.AspNetCore;
using Microsoft.Extensions.Logging;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Infrastructure.WebSockets.Hub;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Ridely.Infrastructure.Services;
internal class WebSocketManager : IWebSocketManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    private readonly ILogger<WebSocketManager> _logger;
    private readonly WebPubSubServiceClient<MainApplicationHub> _webPubSubServiceClient;

    public WebSocketManager(ILogger<WebSocketManager> logger, WebPubSubServiceClient<MainApplicationHub> webPubSubServiceClient)
    {
        _logger = logger;
        _webPubSubServiceClient = webPubSubServiceClient;
    }

    public WebSocket AddWebSocket(string userKey, WebSocket socket)
    {
        _sockets.AddOrUpdate(userKey, socket, (userId, oldSocket) =>
        {
            try
            {
                oldSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server terminated connection", CancellationToken.None);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"Error closing connection {ex.Message} \n {ex.Source}");
                
            }

            return socket;
        });

        return socket;
    }

    public WebSocket? GetWebSocket(string userKey)
    {
        _sockets.TryGetValue(userKey, out var socket);

        return socket;
    }

    public async Task RemoveWebSocket(string userKey)
    {
        _sockets.TryRemove(userKey, out var socket);

        try
        {
            if(socket is not null && socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    "Server terminated connection", CancellationToken.None);
            }
        }
        catch(Exception ex)
        {
            _logger.LogError($"Error closing socket connection. Message: {ex.Message} \n {ex.StackTrace}");
        }
    }

    public async Task<bool> SendMessageAsync(string userKey, string message)
    {
        //WebSocket? webSocket = GetWebSocket(userKey);

        //if (webSocket is null) return false;

        //if(webSocket.State == WebSocketState.Open)
        //{
        //    byte[] messageBytes = Encoding.UTF8.GetBytes(message);

        //    await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);

        //    Console.WriteLine("<---- Message sent ---->");

        //    return true;
        //}
        //else
        //{
        //    await RemoveWebSocket(userKey);

        //    return false;
        //}

        await _webPubSubServiceClient.SendToUserAsync(userKey, message, ContentType.ApplicationJson);

        return true;
    }

    public async Task SendToMultipleAsync(List<string> userKeys, string message)
    {
        foreach(string userKey in userKeys)
            await SendMessageAsync(userKey, message);
    }

    public async Task<bool> SendMessageAsync(WebSocket webSocket, string message)
    {
        if(webSocket.State == WebSocketState.Open)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);

            return true;
        }

        return false;
    }

    public async Task<bool> SendMessageAsync(string userKey, WebSocketResponse<dynamic> message)
    {
        WebSocket? webSocket = GetWebSocket(userKey);

        if (webSocket is null) return false;

        if(webSocket.State == WebSocketState.Open)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            await webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, CancellationToken.None);

            return true;
        }
        else
        {
            await RemoveWebSocket(userKey);

            return false;
        }
    }
}
