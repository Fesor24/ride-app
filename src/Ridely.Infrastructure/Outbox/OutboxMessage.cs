namespace Soloride.Infrastructure.Outbox;
public sealed class OutboxMessage
{
    public OutboxMessage(string type, string content)
    {
        Type = type; 
        Content = content;
        OccurredAtUtc = DateTime.UtcNow;
    }
    
    public long Id { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
}
