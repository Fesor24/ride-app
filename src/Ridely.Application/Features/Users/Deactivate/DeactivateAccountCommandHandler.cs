using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;

namespace Soloride.Application.Features.Users.Deactivate;
internal sealed class DeactivateAccountCommandHandler :
    ICommandHandler<DeactivateAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;

    public DeactivateAccountCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository,
        IRiderRepository riderRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
    }

    public async Task<Result<bool>> Handle(DeactivateAccountCommand request,
        CancellationToken cancellationToken)
    {
        if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            if (rider.IsDeactivated)
                return Error.BadRequest("account.deactivated", "Account not active");

            rider.Deactivate();

            _riderRepository.Update(rider);

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
        else if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository
                .GetAsync(request.DriverId.Value);

            if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

            if (driver.IsDeactivated)
                return Error.BadRequest("account.deactivated", "Account not active");

            driver.Deactivate();

            _driverRepository.Update(driver);

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }

        return false;
    }
}
