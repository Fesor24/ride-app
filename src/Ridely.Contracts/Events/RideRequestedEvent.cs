using Soloride.Contracts.Models;

namespace Soloride.Contracts.Events;
public sealed class RideRequestedEvent
{
    public List<DriverProfile> AvailableDriverProfile { get; init; } = [];
    public RideObject Ride { get; init; }
    public RiderProfile Rider {  get; init; }
}
