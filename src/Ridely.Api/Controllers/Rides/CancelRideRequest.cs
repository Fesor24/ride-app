namespace SolorideAPI.Controllers.Rides;

public class CancelRideRequest
{
    public int RideId { get; set; }
    public string CancellationReason { get; set; }
}
