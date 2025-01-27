using System.ComponentModel.DataAnnotations.Schema;
using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Drivers;
public sealed class DriverWallet : Entity
{
    private DriverWallet()
    {
        
    }

    public DriverWallet(long driverId)
    {
        DriverId = driverId;
    }
    public long DriverId { get; private set; }
    [ForeignKey(nameof(DriverId))]
    public Driver Driver { get; }

    public string? Pin { get; private set; }
    public string? PinResetCode { get; private set; }
    public DateTime? PinResetCodeExpiry { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public decimal TotalBalance { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void DeductBalance(decimal amount)
    {
        AvailableBalance -= amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }
    public void IncremementBalance(decimal amount)
    {
        AvailableBalance += amount;
        TotalBalance += amount;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
