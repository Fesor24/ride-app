using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;

namespace Soloride.Application.Features.Users.UpdateDeviceToken;
internal sealed class UpdateDeviceTokenCommandHandler(IUnitOfWork unitOfWork, 
    IDriverRepository driverRepository, IRiderRepository riderRepository) :
    ICommandHandler<UpdateDeviceTokenCommand>
{
    public async Task<Result<bool>> Handle(UpdateDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        if (request.DriverId.HasValue)
        {
            var driver = await driverRepository
                .GetAsync(request.DriverId.Value);

            if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

            driver.UpdateDeviceToken(request.DeviceTokenId);

            driverRepository.Update(driver);

            return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
        else if (request.RiderId.HasValue)
        {
            var rider = await riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            rider.UpdateDeviceToken(request.DeviceTokenId);

            riderRepository.Update(rider);

            return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
        else return Error.BadRequest("bad.request", "Specify driver or rider");
    }
}
