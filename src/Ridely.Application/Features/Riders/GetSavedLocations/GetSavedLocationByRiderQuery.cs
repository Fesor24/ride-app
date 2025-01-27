using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Riders.GetSavedLocations;
public sealed record GetSavedLocationByRiderQuery(long RiderId) :
    IQuery<List<GetSavedLocationResponse>>;
