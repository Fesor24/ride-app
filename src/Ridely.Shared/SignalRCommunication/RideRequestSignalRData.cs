namespace Ridely.Shared.SignalRCommunication;
public sealed class RideRequestSignalRData
{
    public string Source { get; set; }
    public string Destination { get; set; }
    public List<string> Waypoints { get; set; } = [];
    public long RideId { get; set; }
    public string MusicGenre { get; set; }
    public bool HaveConversation { get; set; }
    public long EstimatedFare { get; set; }
    public string PaymentMethod { get; set; }
    public int ResponseTime {  get; set; }
    public RiderInfo Rider { get; set; }

    public class RiderInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string PhoneNo {  get; set; }
    }
}

public sealed class RideRequestSignalRDriverData
{
    public DriverInfo Driver { get; set; }
    public class DriverInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string PhoneNo { get; set; }
    }
}

public sealed class RideRequestNoMatchSignalRData
{
    public string Message = "No driver found";
}
