namespace RidelyAPI.Controllers.Rides;

public sealed class StartRideRequest
{
    public long RideId { get; set; }
}

public sealed class EndRideRequest
{
    public long RideId { get; set; }
}
