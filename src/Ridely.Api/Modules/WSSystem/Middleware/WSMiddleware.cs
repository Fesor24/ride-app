namespace Modules.ChatSystem.Middleware
{
    //public class WSMiddleware
    //{
    //    private IServiceProvider serviceProvider;
    //    private readonly RequestDelegate next;
    //    private readonly IWSSystemService chatSystemService;
    //    private readonly ILogger<WSMiddleware> logger;
    //    private Dictionary<string, WebSocketRequestHandler> webSocketRequestHandler;


    //    public WSMiddleware(RequestDelegate next, IWSSystemService chatSystemService, ILogger<WSMiddleware> logger)
    //    {
    //        this.next = next;
    //        this.chatSystemService = chatSystemService;
    //        this.logger = logger;
    //        //webSocketRequestHandler = 
    //        webSocketRequestHandler = new Dictionary<string, WebSocketRequestHandler>();
    //    }

    //    public async Task Invoke(HttpContext context, IServiceProvider serviceProvider, IAuthCoreController authCoreController)
    //    {
    //        this.serviceProvider = serviceProvider;
    //        if (context.Request.Path == "/WSDemo")
    //        {

    //            string response = JsonConvert.SerializeObject(new { DateTime.UtcNow, Document = new Docs(serviceProvider) });
    //            context.Response.StatusCode = StatusCodes.Status200OK;
    //            context.Response.ContentType = "application/json";
    //            context.Response.ContentLength = response.Length;
    //            await context.Response.WriteAsync(response);
    //            return;

    //        }



    //        if (!context.WebSockets.IsWebSocketRequest)
    //        {
    //            await next(context);
    //            return;
    //        }
    //        else
    //        {

    //            ClaimsPrincipal? claimsPrincipal = default;
    //            StringValues jwt;
    //            string? jwt_;
    //            context.Request.Headers.TryGetValue("Authorization", out jwt);
    //            if (StringValues.IsNullOrEmpty(jwt))
    //            {
    //                context.Request.Cookies.TryGetValue("Authorization", out jwt_);
    //                if (jwt_ == null)
    //                {
    //                    await next(context);
    //                    return;
    //                }
    //                else
    //                {
    //                    string d = Uri.UnescapeDataString(jwt_);

    //                    claimsPrincipal = authCoreController.GetPrincipalFtomToken(d);
    //                }
    //            }
    //            else
    //            {
    //                claimsPrincipal = authCoreController.GetPrincipalFtomToken(jwt!);
    //            }


    //            if (claimsPrincipal == null)
    //            {
    //                await next(context);
    //                return;
    //            }
    //            //Console.WriteLine("WS Reached");
    //            string? uid = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    //            if (uid == null)
    //            {
    //                await next(context);
    //                return;
    //            }
    //            if (context.Request.Path == "/web-chat")
    //            {
    //                uid += "/web-chat";
    //            }
    //            WebSocket ws = chatSystemService.AddSocket(await context.WebSockets.AcceptWebSocketAsync(), uid);
    //            if (!webSocketRequestHandler.ContainsKey(uid))
    //            {
    //                webSocketRequestHandler.Add(uid, new WebSocketRequestHandler(serviceProvider, uid));
    //            }
    //            await KeepConnectionAlive(ws, uid);

    //        }
    //    }


    //    private Task GetViewResultTask(HttpContext context, string viewName)
    //    {
    //        var viewResult = new ViewResult()
    //        {
    //            ViewName = viewName
    //        };

    //        var executor = context.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>();
    //        var routeData = context.GetRouteData() ?? new Microsoft.AspNetCore.Routing.RouteData();
    //        var actionContext = new ActionContext(context, routeData,
    //        new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
    //        return executor.ExecuteAsync(actionContext, viewResult);
    //    }
    //    private async Task KeepConnectionAlive(WebSocket socket, string userid)
    //    {


    //        while (socket.State == WebSocketState.Open)
    //        {
    //            GeneralEvent<JObject> message = new GeneralEvent<JObject>() { EventName = "End" };
    //            try
    //            {
    //                message = await ReceiveMessageAsync(socket);
    //            }
    //            catch (WebSocketException ex)
    //            {
    //                Console.WriteLine(ex.Message);
    //                break;
    //            }

    //            if (message == null)
    //            {
    //                break;
    //            }
    //            if (message != null)
    //            {
    //                object result = await webSocketRequestHandler[userid].InvokeEvent(message, userid);
    //                if (result == null)
    //                {
    //                    continue;
    //                }

    //                /*if (result is Task) {
    //                    if (result.GetType().IsGenericType)
    //                    {
    //                        object? a = null;
    //                        try {
    //                            a = (Task<dynamic>)result;
    //                        }
    //                        catch(RuntimeBinderException e){}

    //                        if(a != null)
    //                        {
    //                            await chatSystemService.SendMessage(socket, JsonConvert.SerializeObject(a));
    //                        }
    //                    }


    //                }
    //                else
    //                {
    //                    await chatSystemService.SendMessage(socket, JsonConvert.SerializeObject(result));
    //                }*/
    //                await chatSystemService.SendMessage(socket, JsonConvert.SerializeObject(result));

    //            }

    //            if (message != null)
    //            {
    //                //logger.LogInformation($"Received message from ID {userid}: {message}");
    //                //await BroadcastMessageAsync(message);
    //            }
    //        }
    //        JObject params_ = new JObject {
    //            { "UserId", userid }
    //        };
    //        try
    //        {
    //            webSocketRequestHandler[userid].InvokeEvent(new GeneralEvent<JObject>
    //            {
    //                EventName = "Base.Disconnect",
    //                EventArgs = params_
    //            }, userid);
    //        }
    //        catch (KeyNotFoundException ex) { }


    //        await chatSystemService.RemoveSocket(userid);
    //        webSocketRequestHandler.Remove(userid);
    //    }

    //    private async Task<GeneralEvent<JObject?>> ReceiveMessageAsync(WebSocket webSocket)
    //    {
    //        if (webSocket.State == WebSocketState.Closed || webSocket.State == WebSocketState.Closed)
    //        {
    //            return new GeneralEvent<JObject?> { EventName = "End" };
    //        }
    //        var buffer = new byte[1024];
    //        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //        if (result.CloseStatus.HasValue)
    //        {
    //            return null;
    //        }
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
    //            await chatSystemService.SendMessage(webSocket, data);
    //        }

    //        return json;
    //    }


    //}

    //public static class ChatExtentions
    //{
    //    public static IApplicationBuilder UseWSSystem(this IApplicationBuilder app)
    //    {

    //        return app.UseMiddleware<WSMiddleware>();
    //    }

    //    public static void addWSSystem(this IServiceCollection svcs, string? ConnectioString)
    //    {
    //        svcs.AddSingleton<IWSSystemService, WSSystemService>();
    //        svcs.AddMvc();
    //    }

    //}

}
