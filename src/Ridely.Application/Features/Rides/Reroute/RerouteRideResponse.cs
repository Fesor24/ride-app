using Soloride.Domain.Models;

namespace Soloride.Application.Features.Rides.Reroute;
public sealed class RerouteRideResponse
{
    public Location DestinationCoordinates { get; set; }
    public string DestinationAddress { get; set; }
    public int DurationInSeconds { get; set; }
    public DateTime EstimatedTimeOfArrival { get; set; }
}
