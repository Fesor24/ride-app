using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;

namespace Ridely.Application.Features.Rides.Reroute;
public sealed record RerouteRideCommand(
    long RideId,
    string SourceAddress,
    Location Source,
    string DestinationAddress,
    Location Destination) :
    ICommand<RerouteRideResponse>;
