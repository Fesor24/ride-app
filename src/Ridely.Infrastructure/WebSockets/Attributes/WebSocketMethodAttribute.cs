namespace Ridely.Infrastructure.WebSockets.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class WebSocketMethodAttribute : Attribute
{
    public WebSocketMethodAttribute(string name)
    {
        Name = name;
    }
    public string Name { get; init; }
}
