using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public sealed class DriverDiscount : Entity
{
    private DriverDiscount()
    {
        
    }

    public DriverDiscount(long driverId)
    {
        DriverId = driverId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long DriverId { get; private set; }
    public Driver Driver { get; private set; }
    public int Slots { get; private set; }
    public DriverDiscountType Type { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiredAtUtc { get; private set; }

    public void UpdateZeroCommissionDiscount(int days = 7)
    {
        if (Type != DriverDiscountType.ZeroCommissionRide) return;

        if (ExpiredAtUtc > DateTime.UtcNow)
            ExpiredAtUtc.AddDays(days);

        else
            ExpiredAtUtc = DateTime.UtcNow.AddDays(days);
    }
}
