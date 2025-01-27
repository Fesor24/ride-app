using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Riders.GetSavedLocations;
public sealed record GetSavedLocationByRiderQuery(long RiderId) :
    IQuery<List<GetSavedLocationResponse>>;
