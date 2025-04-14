using Amazon.Runtime;

namespace Ridely.Application.Features.Rides.EndRide;
public class EndRideResponse
{
    public long TotalFareAmount { get; set; }
    public long FareOutstanding {  get; set; }
    public long DiscountInPercent { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public string[] Waypoints { get; set; } = [];
    public long WaitingTimeCharge { get; set; } 
}
