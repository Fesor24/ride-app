using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Riders.Get;
public sealed record GetRiderQuery(long RiderId) :
    IQuery<GetRiderResponse>;
