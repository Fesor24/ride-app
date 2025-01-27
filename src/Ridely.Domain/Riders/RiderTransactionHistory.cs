using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Transactions;

namespace Ridely.Domain.Riders;
public sealed class RiderTransactionHistory : Entity
{
    private RiderTransactionHistory()
    {
    }

    public RiderTransactionHistory(long riderId, decimal amount, RiderTransactionType type, 
        Ulid reference, TransactionStatus status)
    {
        RiderId = riderId;
        Amount = amount;
        Status = status;
        Type = type;
        Reference = reference;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Ulid Reference { get; set; }
    public long RiderId { get; set; }
    [ForeignKey(nameof(RiderId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Rider Rider { get; private set; }
    public decimal Amount { get; private set; }
    public string? Error { get; private set; }
    public TransactionStatus Status { get; private set; }
    public RiderTransactionType Type { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void UpdateStatus(TransactionStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
