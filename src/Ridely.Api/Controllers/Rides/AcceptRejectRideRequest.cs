namespace Ridely.Api.Controllers.Rides;

public class AcceptRejectRideRequest
{
    public int RideId { get; set; }
    public bool AcceptRide { get; set; }
}
