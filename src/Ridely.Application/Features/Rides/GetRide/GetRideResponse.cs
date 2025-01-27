namespace Soloride.Application.Features.Rides.GetRide;
public sealed class GetRideResponse
{
    public RideResponseDriverObj Driver { get; set; } = new();
    public RideResponseRideObj Ride { get; set; } = new();
    public RideResponseRiderObj Rider { get; set; } = new();
}

public class RideResponseRideObj
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public decimal Amount { get; set; }
    public decimal EstimatedFare { get; set; }
    public int Rating { get; set; }
    public double DistanceInMeters { get; set; }
    public DateTime Pickup { get; set; }
    public DateTime DropOff { get; set; }
    public string CreatedAt { get; set; }
    public string PaymentMethod { get; set; }
    public int DurationInSeconds { get; set; }
}

public class RideResponseDriverObj
{
    public string Name { get; set; }
    public string ProfileImageUrl { get; set; }
    public RideResponseVehicleObj Cab { get; set; }
}

public class RideResponseRiderObj
{
    public string Name { get; set; }
}

public class RideResponseVehicleObj
{
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
}

