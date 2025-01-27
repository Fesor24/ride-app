using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Rides.Reassign;
public sealed record ReassignRideCommand(int RideId,
    Location Source,
    string SourceAddress,
    string? ReassignReason) :
    ICommand<ReassignRideResponse>;
