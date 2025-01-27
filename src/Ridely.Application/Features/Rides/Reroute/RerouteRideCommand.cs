using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Rides.Reroute;
public sealed record RerouteRideCommand(
    long RideId,
    string SourceAddress,
    Location Source,
    string DestinationAddress,
    Location Destination) :
    ICommand<RerouteRideResponse>;
