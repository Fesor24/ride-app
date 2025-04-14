using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.GetWaitTime;
public sealed record GetWaitTimeQuery() : IQuery<IReadOnlyList<GetWaitTimeResponse>>;
