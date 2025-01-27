using Ridely.Application.Models.WebSocket;
using System.Net.WebSockets;

namespace Ridely.Application.Abstractions.Websocket;
public interface IWebSocketManager
{
    WebSocket AddWebSocket(string userKey, WebSocket socket);
    WebSocket? GetWebSocket(string userKey);
    Task RemoveWebSocket(string userKey);
    Task<bool> SendMessageAsync(string userKey, string message);
    Task<bool> SendMessageAsync(WebSocket webSocket, string message);
    Task<bool> SendMessageAsync(string userKey, WebSocketResponse<dynamic> message);
    Task SendToMultipleAsync(List<string> userKeys, string message);
}
