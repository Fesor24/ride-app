using MediatR;
using Ridely.Application.Abstractions.Storage;
using Ridely.Domain.Abstractions;
using DriverEntity = Ridely.Domain.Drivers.Driver;
using Ridely.Domain.Drivers.Events;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.Helper;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Microsoft.Extensions.Logging;

namespace Ridely.Application.Features.Drivers.RegisterDriver;
internal sealed class DriverRegisteredDomainEventHandler :
    INotificationHandler<DriverRegisteredDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectStoreService _objectStoreService;
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverReferrersRepository _driverReferrersRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderReferrersRepository _riderReferrersRepository;
    private readonly ILogger<DriverRegisteredDomainEventHandler> _logger;

    public DriverRegisteredDomainEventHandler(IUnitOfWork unitOfWork,
        IObjectStoreService objectStoreService, IDriverRepository driverRepository,
        IDriverReferrersRepository driverReferrersRepository, IRiderRepository riderRepository,
        IRiderReferrersRepository riderReferrersRepository, ILogger<DriverRegisteredDomainEventHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _objectStoreService = objectStoreService;
        _driverRepository = driverRepository;
        _driverReferrersRepository = driverReferrersRepository;
        _riderRepository = riderRepository;
        _riderReferrersRepository = riderReferrersRepository;
        _logger = logger;
    }

    public async Task Handle(DriverRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository
            .GetByPhoneNoAsync(notification.PhoneNo);

        if (driver is null)
        {
            _logger.LogInformation("Driver was null");

            return;
        };

        _logger.LogInformation("Driver from db {PhoneNo}", driver.PhoneNo);

        await UploadImagesToStore(driver.Id,
            notification.ProfileImageBase64, notification.DriversLicenseBase64);

        _logger.LogInformation("Uploaded images to s3");

        await UpdateImagesInDatabase(driver);

        await GenerateReferrerCodeAndHandleReferrer(notification.RefferedBy, driver);

        _driverRepository.Update(driver);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task GenerateReferrerCodeAndHandleReferrer(string refferedByCode, DriverEntity driver)
    {
        string firstName = driver.FirstName;

        if (firstName.Length < 3)
            firstName = firstName.PadRight(3, '0');
        else
            firstName = firstName[..3];

        string referralCode = firstName + "-sd" + RandomGenerator.GenerateReferralCode(driver.Id);

        driver.UpdateReferralCode(referralCode);

        if (string.IsNullOrWhiteSpace(refferedByCode)) return;

        string[] referredBySplit = refferedByCode.Split("-");

        if (referredBySplit.Length != 2) return;

        string referCodeQualifier = referredBySplit[1];

        if (referCodeQualifier.StartsWith("sd", StringComparison.InvariantCultureIgnoreCase))
        {
            // driver was the person that referred this user
            var referredByDriver = await _driverRepository
                .GetByReferralCodeAsync(refferedByCode);

            if (referredByDriver is null) return;

            DriverReferrers driverReferrers = new(referredByDriver.Id,
                driver.Id, Domain.Shared.ReferredUser.Driver);

            await _driverReferrersRepository.AddAsync(driverReferrers);

            driver.UpdateReferredBy(referredByDriver.Id, Domain.Shared.ReferredUser.Driver);
        }

        else if (referCodeQualifier.StartsWith("sr", StringComparison.InvariantCultureIgnoreCase))
        {
            // rider was the person that referred this user...
            var referredByRider = await _riderRepository
                .GetByReferralCodeAsync(refferedByCode);

            if (referredByRider is null) return;

            RiderReferrers riderReferrers = new(referredByRider.Id,
                driver.Id, Domain.Shared.ReferredUser.Driver);

            await _riderReferrersRepository.AddAsync(riderReferrers);

            driver.UpdateReferredBy(referredByRider.Id, Domain.Shared.ReferredUser.Rider);
        }
    }

    private async Task UpdateImagesInDatabase(DriverEntity driver)
    {
        string driversLicenseKey = UploadKeys.Driver.DriversLicense(driver.Id);

        DateTime expiry = DateTime.UtcNow.AddDays(7);

        (string licenseImageUrl, DateTime licenseExpiry) = await _objectStoreService
            .GeneratePreSignedUrl(driversLicenseKey);

        string driversProfileImageKey = UploadKeys.Driver.ProfileImage(driver.Id);

        (string profileImageUrl, DateTime profileImageExpiry) = await _objectStoreService
            .GeneratePreSignedUrl(driversProfileImageKey);

        driver.UploadImages(profileImageUrl, licenseImageUrl, licenseExpiry);
    }

    private async Task UploadImagesToStore(long driverId, string profileImageBase64, string driversLicenseBase64)
    {
        string driversLicenseKey = UploadKeys.Driver.DriversLicense(driverId);

        var uploadLicenseResponse = await _objectStoreService.UploadAsync(driversLicenseKey, driversLicenseBase64);

        string driversProfileImageKey = UploadKeys.Driver.ProfileImage(driverId);

        var uploadProfileImageResponse = await _objectStoreService.UploadAsync(driversProfileImageKey, profileImageBase64);
    }
}
