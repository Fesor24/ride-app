using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.StartRide;
public sealed class StartRideResponse
{
    public RideLocation Source { get; set; }
    public RideLocation Destination { get; set; }
    public List<RideLocation> Waypoints { get; set; } = [];

    public MusicGenre MusicGenre { get; set; }
    public bool? RideConversation { get; set; }
    public PaymentMethod PaymentMethod { get; set; }

    public class RideLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
    }
}
