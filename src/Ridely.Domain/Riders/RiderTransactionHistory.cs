using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;
using Ridely.Domain.Transactions;

namespace Ridely.Domain.Riders;
public sealed class RiderTransactionHistory : Entity
{
    private RiderTransactionHistory()
    {
    }

    public RiderTransactionHistory(long riderId, decimal amount, RiderTransactionType type, 
        Ulid reference, TransactionStatus status, PaymentProvider paymentProvider, long? rideId = null)
    {
        RiderId = riderId;
        Amount = amount;
        Status = status;
        Type = type;
        Reference = reference;
        CreatedAtUtc = DateTime.UtcNow;
        RideId = rideId;
        PaymentProvider = paymentProvider;
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
    public PaymentProvider PaymentProvider { get; private set; }
    public long? RideId { get; private set; }
    [ForeignKey(nameof(RideId))]
    public Ride Ride { get; private set; }
    public string RidePaymentReferences { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    public void UpdateStatus(TransactionStatus status)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    public void SetRide(long rideId)
    {
        RideId = rideId;
    }

    public void SetError(string error)
    {
        Error = error;
    }

    public void SetRidePaymentReference(List<string> references)
    {
        if (references.Count == 0) return;

        RidePaymentReferences = string.Join(".", references);
    }
}
