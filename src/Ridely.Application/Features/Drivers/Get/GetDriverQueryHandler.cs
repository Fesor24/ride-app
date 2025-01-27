using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;

namespace Soloride.Application.Features.Drivers.Get;
internal sealed class GetDriverQueryHandler :
    IQueryHandler<GetDriverQuery, GetDriverResponse>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverWalletRepository _driverWalletRepository;
    private readonly IDriverReferrersRepository _driverReferrersRepository;

    public GetDriverQueryHandler(IDriverRepository driverRepository, IDriverWalletRepository driverWalletRepository,
        IDriverReferrersRepository driverReferrersRepository)
    {
        _driverRepository = driverRepository;
        _driverWalletRepository = driverWalletRepository;
        _driverReferrersRepository = driverReferrersRepository;
    }

    public async Task<Result<GetDriverResponse>> Handle(GetDriverQuery request,
        CancellationToken cancellationToken)
    {
        var driver = await _driverRepository
            .GetDetailsAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        var referredUsers = await _driverReferrersRepository
            .GetReferredUsersCount(driver.Id);

        var wallet = await _driverWalletRepository
            .GetByDriverAsync(driver.Id);

        return new GetDriverResponse
        {
            Driver = new()
            {
                ProfileImageUrl = driver.ProfileImageUrl,
                LicenseImageUrl = driver.LicenseImageUrl,
                FirstName = driver.FirstName,
                LastName = driver.LastName,
                LicenseNo = driver.LicenseNo,
                ZeroCommissionTripsExpiry = driver.ZeroCommissionRidesExpiry,
                Status = driver.Status,
                DeviceTokenId = driver.DeviceTokenId,
                AvgRatings = driver.AvgRatings,
                CompletedRides = driver.CompletedTrips,
                IdentityValidated = driver.IdentityValidated,
                Email = driver.Email,
                PhoneNo = driver.PhoneNo,
                AvailableBalance = wallet!.AvailableBalance
            },
            Cab = new()
            {
                Manufacturer = driver.Cab.Manufacturer,
                Model = driver.Cab.Model,
                Color = driver.Cab.Color,
                Name = driver.Cab.Name,
                LicensePlateNo = driver.Cab.LicensePlateNo,
                Year = driver.Cab.Year
            },
            ReferralInfo = new()
            {
                ReferralCode = string.IsNullOrWhiteSpace(driver.ReferralCode) ? "" :
                    driver.ReferralCode.ToUpperInvariant(),
                DriversReferred = referredUsers.Drivers,
                RidersReferred = referredUsers.Riders
            }
        };
    }
}
