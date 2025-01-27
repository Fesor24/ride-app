namespace SolorideAPI.WebSocket.Controller;

public class WebSocketControllerBase
{
    public string UserIdentifier { get; set; }

    public virtual void Disconnect(string userIdentifier) =>
        Console.WriteLine("Disconnected: User Identifier: " + userIdentifier);
}
