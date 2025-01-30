using Ridely.Domain.Rides;

namespace Ridely.Api.Controllers.Rides;

public sealed class UpdatePaymentToCashRequest
{
    public long RideId { get; set; }
    public PaymentMethod Method { get; set; }
    public long? PaymentCardId { get; set; }
}
