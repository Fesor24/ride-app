using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Riders.Get;
public sealed record GetRiderQuery(long RiderId) :
    IQuery<GetRiderResponse>;
