namespace Ridely.Infrastructure.Route;

public sealed class GoogleRouteOptions
{
    public const string NAME = "GoogleRoute";
    public string BaseAddress { get; init; }
    public string ApiKey { get; init; }
}
