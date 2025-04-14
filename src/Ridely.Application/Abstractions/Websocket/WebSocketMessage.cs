using Ridely.Shared.Helper;

namespace Ridely.Application.Abstractions.Websocket;

public class WebSocketMessage<T>
{
    private WebSocketMessage(string eventName, T payload)
    {
        Event = eventName;
        Payload = payload;
    }

    public string Event { get; private set; }
    public T Payload { get; private set; }

    public static string Create(string eventName, T payload)
    {
        var wsMessage = new WebSocketMessage<T>(eventName, payload);

        return Serialize.Object(wsMessage);
    }
        
}
