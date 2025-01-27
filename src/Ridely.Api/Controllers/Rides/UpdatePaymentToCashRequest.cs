using Soloride.Domain.Rides;

namespace SolorideAPI.Controllers.Rides;

public sealed class UpdatePaymentToCashRequest
{
    public long RideId { get; set; }
    public PaymentMethod Method { get; set; }
    public long? PaymentCardId { get; set; }
}
