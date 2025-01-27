namespace Ridely.Application.Features.Rides.EndRide;
public class EndRideResponse
{
    public long Fare { get; set; }
    public long FareDueByRider {  get; set; } 
    public string Source { get; set; }
    public string Destination { get; set; }
    public bool Paid { get; set; } = false;
}
