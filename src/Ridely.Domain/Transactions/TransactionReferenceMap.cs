using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Transactions;
public sealed class TransactionReferenceMap : Entity
{
    private TransactionReferenceMap()
    {
        
    }

    public TransactionReferenceMap(Ulid reference, TransactionReferenceType type)
    {
        Reference = reference;
        Type = type;
    }

    public Ulid Reference { get; private set; }
    public TransactionReferenceType Type { get; private set; }
}
