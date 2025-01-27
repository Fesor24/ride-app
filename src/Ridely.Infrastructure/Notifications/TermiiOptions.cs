using Soloride.Application.Abstractions.Notifications;
public sealed class TermiiOptions
{
    public const string NAME = "Termii";
    public string BaseAddress { get; init; }
    public string ApiKey { get; init; }
}
