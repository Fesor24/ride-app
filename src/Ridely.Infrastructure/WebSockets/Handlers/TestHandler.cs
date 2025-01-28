using Ridely.Infrastructure.WebSockets.Attributes;

namespace Ridely.Infrastructure.WebSockets.Handlers;

[WebSocketClass("TEST")]
public class TestHandler : IWebSocketEventHandlerMarker
{
    [WebSocketMethod("HELLO")]
    public void Hello(string name)
    {
        Console.WriteLine("Hello " + name);
    }
}
