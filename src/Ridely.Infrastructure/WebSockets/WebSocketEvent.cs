namespace Ridely.Infrastructure.WebSockets;
public sealed class WebSocketEvent
{
    public WebSocketEvent(string eventName)
    {
        EventName = eventName;
    }

    public string EventName { get; init; }
    public Dictionary<string, object> Payload { get; set; } = new();
}
