using Soloride.Domain.Rides;

namespace SolorideAPI.Controllers.Rides;

public class RideRequest
{
    public long RideId { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public MusicGenre MusicGenre { get; set; }
    public RideCategory RideCategory { get; set; }
    public bool? RideConversation { get; set; }
    public long? PaymentCardId { get; set; }
}

public class ReassignRideRequest
{
    public int RideId { get; set; }
    public string? RerouteReason { get; set; } = "";
    public RideLocation Source { get; set; }
}

public class RerouteRideRequest
{
    public long RideId { get; set; }
    public RideLocation Source { get; set; }
    public RideLocation Destination { get; set; }
}