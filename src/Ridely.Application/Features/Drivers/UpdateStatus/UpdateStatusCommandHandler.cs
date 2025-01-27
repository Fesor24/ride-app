using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Drivers.UpdateStatus;
internal sealed class UpdateStatusCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository) :
    ICommandHandler<UpdateStatusCommand>
{
    public async Task<Result<bool>> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var driver = await driverRepository
            .GetAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        if (driver.CurrentRideId.HasValue)
            return Error.BadRequest("driver.inride", "Driver currently in a ride");

        if (!(driver.Status == DriverStatus.Offline || driver.Status == DriverStatus.Online))
            return Error.BadRequest("bad.request", "Driver is not offline or online");

        DriverStatus status = DriverStatus.Offline;

        if (request.Status == UpdateDriverStatusEnum.Online)
            status = DriverStatus.Online;

        driver.UpdateStatus(status);

        driverRepository.Update(driver);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
