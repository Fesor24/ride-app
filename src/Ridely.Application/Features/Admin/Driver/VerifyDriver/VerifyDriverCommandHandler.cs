using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Admin.Driver.VerifyDriver;
internal sealed class VerifyDriverCommandHandler :
    ICommandHandler<VerifyDriverCommand>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyDriverCommandHandler(IDriverRepository driverRepository, IUnitOfWork unitOfWork)
    {
        _driverRepository = driverRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(VerifyDriverCommand request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByPhoneNoAsync(request.PhoneNo);

        if (driver is null)
            return Error.NotFound("driver.notfound", "Driver not found");

        if (driver.IdentityValidated)
            return Error.BadRequest("identity.validated", "Driver identity validated");

        driver.ValidateIdentity();

        _driverRepository.Update(driver);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
