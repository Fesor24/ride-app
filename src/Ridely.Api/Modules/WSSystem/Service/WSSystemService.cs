using Microsoft.EntityFrameworkCore;
using Modules.ChatSystem.Models;
using Modules.ChatSystem.Service;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace Modules.ChatSystem.Service
{
    public class WSSystemService : IWSSystemService
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private readonly ILogger<WSSystemService> logger;
        public WSSystemService(IServiceScopeFactory serviceScopeFactory, ILogger<WSSystemService> logger) {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                //this.dbContext = scope.ServiceProvider.GetService<ChatDBContext>();
            }

            this.logger = logger;
        }


        

        public WebSocket AddSocket(WebSocket socket, string userid)
        {
            _sockets.AddOrUpdate(userid, socket, (userid, old)=>{
                try
                {
                    old.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server terminated connection", CancellationToken.None);
                    
                }
                catch (Exception e){
                    logger.LogCritical($"Error Closing Connection {e.Message}\n {e.Source}");
                }

                return socket;
            });

            return socket;
        }

        

        

        public WebSocket? GetSocket(string userid)
        {
            _sockets.TryGetValue(userid, out var socket);
            return socket;
        }


        public async Task RemoveSocket(string socketId)
        {
            WebSocket _;
            _sockets.TryRemove(socketId, out _);
            try
            {
                if (_ != null) {
                    await _.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server terminated connection", CancellationToken.None);
                }
                
            }catch (Exception ex)
            {

            }
           
        }

        public async Task<bool?> SendMessage(string userid, string message)
        {
            WebSocket? socket = GetSocket(userid);
            if (socket == null) { Console.WriteLine("User Not Connected: " + userid); return false; }
            if (socket.State == WebSocketState.Open)
            {
                Console.WriteLine("Sent: " + message);
                 await socket.SendAsync(System.Text.Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, 
                     CancellationToken.None);
                return true;
            }
            else
            {
                Console.WriteLine("Removing User: " + userid);
                await RemoveSocket(userid);
                return false;
            }
               
        }

        public async Task<bool?> SendMessage(WebSocket socket, string message)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(System.Text.Encoding.UTF8.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            return false;
        }

        public async Task<bool?> SendMessage(string userid, WebsocketReply<dynamic> message)
        {
            if (userid == null) { return false; }
            WebSocket? socket = GetSocket(userid);
            if (socket == null) { return false; }
            if (socket.State == WebSocketState.Open)
            {
                Console.WriteLine("Sent: " + userid);
                await socket.SendAsync(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            else
            {
                Console.WriteLine("Removing User: "+userid);
                await RemoveSocket(userid);
                return false;
            }
        }
    }
}
