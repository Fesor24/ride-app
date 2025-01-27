using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Transactions;
public sealed class TransactionLog : Entity
{
    private TransactionLog()
    {
        
    }

    public TransactionLog(Ulid reference, string content, TransactionLogEvent type)
    {
        Reference = reference;
        Content = content;
        CreatedAtUtc = DateTime.UtcNow;
        Event = type;
    }

    public Ulid Reference { get; private set; }
    public string Content { get; private set; }
    public TransactionLogEvent Event { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
