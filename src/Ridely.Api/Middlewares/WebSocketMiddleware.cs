using Microsoft.Extensions.Primitives;
using Modules.ChatSystem.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using RidelyAPI.WebSocket.Handler;

namespace RidelyAPI.Middlewares;

//public class WebSocketMiddleware
//{
//    private readonly RequestDelegate _next;
//    private IServiceProvider _serviceProvider;
//    private readonly IWebSocketManager _webSocketManager;
//    private readonly Dictionary<string, WebSocketRequestHandler> _webSocketRequestHandler;
//    public WebSocketMiddleware(RequestDelegate next, IWebSocketManager webSocketManager)
//    {
//        _next = next;
//        _webSocketManager = webSocketManager;
//        _webSocketRequestHandler = new();
//    }

//    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider, 
//        IJwtService tokenService)
//    {
//        _serviceProvider = serviceProvider;

//        if (!context.WebSockets.IsWebSocketRequest)
//        {
//            await _next(context);

//            return;
//        }

//        IDictionary<string, object> claims;

//        bool isRider = false;

//        StringValues jwt;

//        string? jwt_;

//        context.Request.Headers.TryGetValue("Authorization", out jwt);

//        if (StringValues.IsNullOrEmpty(jwt))
//        {
//            context.Request.Cookies.TryGetValue("Authorization", out jwt_);

//            if(jwt_ == null)
//            {
//                await _next(context);

//                return;
//            }
//            else
//            {
//                string token = Uri.UnescapeDataString(jwt_);

//                claims = await tokenService.GetClaimsFromToken(token);
//            }
//        }
//        else
//        {
//            claims = await tokenService.GetClaimsFromToken(jwt!);
//        }

//        if (claims.ContainsKey(ClaimsConstant.Driver))
//            isRider = false;

//        else if (claims.ContainsKey(ClaimsConstant.Rider))
//            isRider = true;

//        else
//        {
//            await _next(context);

//            return;
//        }

//        string? identifier = "";

//        string webSocketUserKey = "";

//        if (isRider)
//            identifier = tokenService.GetClaimValueFromToken<string>(ClaimsConstant.Rider, claims);

//        else
//            identifier = tokenService.GetClaimValueFromToken<string>(ClaimsConstant.Driver, claims);

//        if (string.IsNullOrWhiteSpace(identifier))
//        {
//            await _next(context);

//            return;
//        }

//        if (isRider)
//            webSocketUserKey = WebSocketKeys.Rider.Key(identifier);

//        else
//            webSocketUserKey = WebSocketKeys.Driver.Key(identifier);

//        //if (context.Request.Path == "/web-chat")
//        //    identifier += "/web-chat";

//        if (context.Request.Path == "/web-chat")
//            webSocketUserKey += "/web-chat";

//        System.Net.WebSockets.WebSocket webSocket = _webSocketManager
//            .AddWebSocket(webSocketUserKey, await context.WebSockets.AcceptWebSocketAsync());

//        if (!_webSocketRequestHandler.ContainsKey(webSocketUserKey))
//            _webSocketRequestHandler.Add(webSocketUserKey, new WebSocketRequestHandler(serviceProvider, webSocketUserKey));

//        await KeepConnectionAlive(webSocketUserKey, webSocket);
//    }

//    private async Task KeepConnectionAlive(string webSocketUserKey, System.Net.WebSockets.WebSocket webSocket)
//    {
//        while(webSocket.State == System.Net.WebSockets.WebSocketState.Open)
//        {
//            GeneralEvent<JObject> message = new GeneralEvent<JObject>() { EventName = "End" };

//            try
//            {
//                message = await ReceiveMessageAsync(webSocket);
//            }
//            catch(Exception ex)
//            {
//                // Log error
//                break;
//            }

//            if (message == null)
//                break;

//            else
//            {
//                object result = await _webSocketRequestHandler[webSocketUserKey].InvokeAsync(message, webSocketUserKey);

//                if (result == null)
//                    continue;

//                await _webSocketManager.SendMessageAsync(webSocket, System.Text.Json.JsonSerializer.Serialize(result));
//            }
//        }

//        JObject params_ = new()
//        {
//            {"userIdentifier", webSocketUserKey }
//        };

//        try
//        {
//            await _webSocketRequestHandler[webSocketUserKey].InvokeAsync(new GeneralEvent<JObject>
//            {
//                EventName = "DRIVER.DISCONNECT",
//                EventArgs = params_
//            }, webSocketUserKey);
//        }
//        catch(Exception ex)
//        {

//        }

//        await _webSocketManager.RemoveWebSocket(webSocketUserKey);

//        _webSocketRequestHandler.Remove(webSocketUserKey);
//    }

//    private async Task<GeneralEvent<JObject>> ReceiveMessageAsync(System.Net.WebSockets.WebSocket webSocket)
//    {
//        if (webSocket.State == System.Net.WebSockets.WebSocketState.Closed)
//            return new GeneralEvent<JObject>
//            {
//                EventName = "End"
//            };

//        var buffer = new byte[1024];

//        var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

//        if (result.CloseStatus.HasValue) return null;

//        string data;

//        GeneralEvent<JObject?>? json = null;

//        try
//        {
//            data = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
//            json = JsonConvert.DeserializeObject<GeneralEvent<JObject?>>(data);
//        }
//        catch (Exception e)
//        {
//            data = JsonConvert.SerializeObject(new GeneralEvent<JObject?>
//            {
//                EventName = "Error",
//                EventArgs = new JObject { { "Message", e.Message } }

//            });

//            await _webSocketManager.SendMessageAsync(webSocket, data);
//        }

//        return json;
//    }
//}
