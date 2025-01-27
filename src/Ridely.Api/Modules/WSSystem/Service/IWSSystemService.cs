using Modules.ChatSystem.Models;
using System.Net.WebSockets;

namespace Modules.ChatSystem.Service
{
    public interface IWSSystemService
    {
        public WebSocket AddSocket(WebSocket socket, string userid);
        public WebSocket? GetSocket(string userid);
        public Task<bool?> SendMessage(string userid, string message);
        public Task<bool?> SendMessage(string userid, WebsocketReply<dynamic> message);
        public Task<bool?> SendMessage(WebSocket socket, string message);
        public Task RemoveSocket(string socketId);
    }
}
