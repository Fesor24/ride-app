namespace Ridely.Api.Controllers.Rides;

public class RideFareEstimateRequest
{
    public RideLocation Source { get; set; }
    public RideLocation Destination { get; set; }
}

public class RideLocation
{
    public double Long { get; set; }
    public double Lat { get; set; }
    public string Address { get; set; }
}
