namespace Ridely.Application.Features.Rides.Search;
public sealed class SearchRideResponse
{
    public long Id { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public decimal Amount { get; set; }
    public string CreatedAt { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
}
