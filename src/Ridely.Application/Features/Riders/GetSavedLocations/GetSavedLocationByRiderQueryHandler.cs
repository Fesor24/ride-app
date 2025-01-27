using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;

namespace Soloride.Application.Features.Riders.GetSavedLocations;
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
