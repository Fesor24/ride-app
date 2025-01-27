using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.GetSavedLocations;
internal sealed class GetSavedLocationByRiderQueryHandler(ISavedLocationRepository savedLocationRepository) :
    IQueryHandler<GetSavedLocationByRiderQuery, List<GetSavedLocationResponse>>
{
    public async Task<Result<List<GetSavedLocationResponse>>> Handle(GetSavedLocationByRiderQuery request,
        CancellationToken cancellationToken)
    {
        var savedLocations = await savedLocationRepository
            .GetByRiderAsync(request.RiderId);

        return savedLocations.ToList().ConvertAll(location => new GetSavedLocationResponse
        {
            Address = location.Address,
            Coordinates = location.Coordinates,
            Id = location.Id,
        });
    }
}
