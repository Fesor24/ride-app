using System.ComponentModel.DataAnnotations.Schema;
using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Riders;
public sealed class RiderWallet : Entity
{
    private RiderWallet()
    {
        
    }

    public RiderWallet(long riderId)
    {
        RiderId = riderId;
    }

    public long RiderId { get; private set; }
    [ForeignKey(nameof(RiderId))]
    public Rider Rider { get; }

    public string? Pin { get; private set; }
    public string? PinResetCode { get; private set; }
    public DateTime? PinResetCodeExpiry { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public decimal TotalBalance { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void DeductBalance(decimal balance)
    {
        AvailableBalance -= balance;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void IncrementBalance(decimal amount)
    {
        AvailableBalance += amount;
        TotalBalance += amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
