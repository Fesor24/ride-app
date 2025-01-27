using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;

namespace Soloride.Application.Features.Riders.DeleteSavedLocation;
internal sealed class DeleteSavedLocationCommandHandler :
    ICommandHandler<DeleteSavedLocationCommand>
{
    private readonly ISavedLocationRepository _savedLocationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSavedLocationCommandHandler(ISavedLocationRepository savedLocationRepository,
        IUnitOfWork unitOfWork)
    {
        _savedLocationRepository = savedLocationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteSavedLocationCommand request, CancellationToken cancellationToken)
    {
        var savedLocation = await _savedLocationRepository
            .GetAsync(request.SavedLocationId);

        if (savedLocation is null)
            return Error.NotFound("location.notfound", "Saved location not found");

        _savedLocationRepository.Delete(savedLocation);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
