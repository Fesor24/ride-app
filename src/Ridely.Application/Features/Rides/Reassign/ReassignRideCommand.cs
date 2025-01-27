using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;

namespace Ridely.Application.Features.Rides.Reassign;
public sealed record ReassignRideCommand(int RideId,
    Location Source,
    string SourceAddress,
    string? ReassignReason) :
    ICommand<ReassignRideResponse>;
