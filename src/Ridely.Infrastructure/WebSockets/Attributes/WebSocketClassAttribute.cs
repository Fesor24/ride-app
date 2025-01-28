namespace Ridely.Infrastructure.WebSockets.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class WebSocketClassAttribute : Attribute
{
    public WebSocketClassAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
}
