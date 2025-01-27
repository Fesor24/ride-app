using Ridely.Contracts.Models;

namespace Ridely.Contracts.Events;
public sealed class RideRequestedEvent
{
    public List<DriverProfile> AvailableDriverProfile { get; init; } = [];
    public RideObject Ride { get; init; }
    public RiderProfile Rider {  get; init; }
}
