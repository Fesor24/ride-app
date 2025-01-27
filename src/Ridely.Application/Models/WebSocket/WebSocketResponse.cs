namespace Soloride.Application.Models.WebSocket;

public class WebSocketResponse<T>
{
    public string? Event { get; set; }
    public T? Payload { get; set; }
}
