using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.RequestWaitTime;
public sealed record RequestWaitTimeCommand(long RideId, long WaitTimeId) : ICommand<RequestWaitTimeResponse>;
