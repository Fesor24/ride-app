﻿using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.UpdateSavedLocation;
internal sealed class UpdateSavedLocationCommandHandler(IUnitOfWork unitOfWork, ISavedLocationRepository savedLocationRepository) :
    ICommandHandler<UpdateSavedLocationCommand>
{
    public async Task<Result<bool>> Handle(UpdateSavedLocationCommand request, CancellationToken cancellationToken)
    {
        var savedLocation = await savedLocationRepository
            .GetAsync(request.SavedLocationId);

        if (savedLocation is null)
            return Error.NotFound("location.notfound", "Saved location not found");

        savedLocation.UpdateAddress(request.Address, request.Coordinates.Latitude, request.Coordinates.Longitude);

        savedLocationRepository.Update(savedLocation);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
