using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Riders;
public sealed class RiderDiscount : Entity
{
    private RiderDiscount()
    {
        
    }

    public long RiderId { get; private set; }
    public Rider Rider { get; private set; }
    public int DiscountInPercent { get; private set; }
    public int Slots { get; private set; }
    public RiderDiscountType Type { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? ExpiredAtUtc { get; private set; }
}
