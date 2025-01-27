namespace Ridely.Application.Features.Admin.Rides.GetById;
public sealed class GetRideResponse
{
    public long RideId { get; set; }
    public string Driver { get; set; }
    public string Rider { get; set; }
    public decimal Amount { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}
