using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.AcceptRejectWaitTime;
public sealed record AcceptRejectWaitTimeCommand(long RideId, long RideLogId,
    bool AcceptWaitTimeExtension, string? Reason) : ICommand;
