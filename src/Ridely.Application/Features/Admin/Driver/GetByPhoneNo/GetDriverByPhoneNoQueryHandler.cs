using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;

namespace Soloride.Application.Features.Admin.Driver.GetByPhoneNo;
internal sealed class GetDriverByPhoneNoQueryHandler :
    IQueryHandler<GetDriverByPhoneNoQuery, GetDriverByPhoneNoResponse>
{
    private readonly IDriverRepository _driverRepository;

    public GetDriverByPhoneNoQueryHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<Result<GetDriverByPhoneNoResponse>> Handle(GetDriverByPhoneNoQuery request,
        CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByPhoneNoAsync(request.PhoneNo);

        if (driver is null)
            return Error.NotFound("driver.notfound", "Driver not found");

        return new GetDriverByPhoneNoResponse
        {
            Email = driver.Email,
            PhoneNo = driver.PhoneNo,
            FirstName = driver.FirstName,
            LastName = driver.LastName,
            ProfileImageUrl = driver.ProfileImageUrl,
            LicenseImageUrl = driver.LicenseImageUrl,
            IdentityValidated = driver.IdentityValidated,
            LicenseNo = driver.LicenseNo,
            Cab = new CabResponse
            {
                Color = driver.Cab.Color,
                Manufacturer = driver.Cab.Manufacturer,
                Model = driver.Cab.Model,
                Year = driver.Cab.Year
            }
        };
    }
}
