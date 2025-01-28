namespace Ridely.Infrastructure.WebSockets;
internal sealed class WebSocketEvent
{
    public string EventName { get; set; }
    public Dictionary<string, object> Payload { get; set; }
}
