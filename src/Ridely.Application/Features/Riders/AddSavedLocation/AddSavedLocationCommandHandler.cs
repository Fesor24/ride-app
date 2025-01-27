using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.AddSavedLocation;
internal sealed class AddSavedLocationCommandHandler :
    ICommandHandler<AddSavedLocationCommand>
{
    private readonly IRiderRepository _riderRepository;
    private readonly ISavedLocationRepository _savedLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSavedLocationCommandHandler(IRiderRepository riderRepository, ISavedLocationRepository savedLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _riderRepository = riderRepository;
        _savedLocationRepository = savedLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AddSavedLocationCommand request, CancellationToken cancellationToken)
    {
        var rider = await _riderRepository
            .GetAsync(request.RiderId);

        if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

        var savedLocationByType = await _savedLocationRepository
            .GetByRiderAndLocationTypeAsync(rider.Id, request.LocationType);

        if (savedLocationByType is not null)
            return Error.BadRequest("location.exist", $"Saved location with type ({request.LocationType.ToString()}) exist");

        SavedLocation savedLocation = new(
            rider.Id,
            request.LocationType,
            request.Coordinates.Latitude,
            request.Coordinates.Longitude,
            request.Address
            );

        await _savedLocationRepository.AddAsync(savedLocation);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
