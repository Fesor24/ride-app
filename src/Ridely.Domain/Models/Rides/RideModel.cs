using Ridely.Domain.Rides;

namespace Ridely.Domain.Models.Rides;
public class RideModel : BaseModel
{
    public string Rider {  get; set; }
    public string Driver { get; set; }

    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public RideStatus RideStatus { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public DateTime CreatedAt { get; set; }
}
