﻿using Hangfire;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Storage;
using Ridely.Application.Extensions;
using Ridely.Application.Features.Accounts;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Services;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Users.PhoneNoVerification;
internal sealed class NumberVerificationCommandHandler:
    ICommandHandler<NumberVerificationCommand, NumberVerificationResponse>
{
    private readonly ICacheService _cacheService;
    private readonly IRiderRepository _riderRepository;
    private readonly IJwtService _jwtService;
    private readonly IDriverRepository _driverRepository;
    private readonly IObjectStoreService _objectStoreService;
    private readonly IUnitOfWork _unitOfWork;

    public NumberVerificationCommandHandler(ICacheService cacheService,
        IRiderRepository riderRepository, IJwtService jwtService,
        IDriverRepository driverRepository, IObjectStoreService objectStoreService,
        IUnitOfWork unitOfWork)
    {
        _cacheService = cacheService;
        _riderRepository = riderRepository;
        _jwtService = jwtService;
        _driverRepository = driverRepository;
        _objectStoreService = objectStoreService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NumberVerificationResponse>> Handle(NumberVerificationCommand request,
        CancellationToken cancellationToken)
    {
        string key = Cache.UserAuth.Key(request.PhoneNo.ToPhoneNumber(), (int)request.AppInstance);

        string? code = await _cacheService.GetAsync(key);

        if (string.IsNullOrWhiteSpace(code) || code != request.Code)
            return new NumberVerificationResponse
            {
                CodeVerificationStatus = false
            };

        string accessToken, refreshToken;

        if (request.AppInstance == ApplicationInstance.Rider)
        {
            var rider = await _riderRepository
                .GetByPhoneNoAsync(request.PhoneNo.ToPhoneNumber());

            if (rider is null)
                return new NumberVerificationResponse
                {
                    CodeVerificationStatus = true,
                    NewUser = true
                };

            if (rider.IsDeactivated)
                return Error.BadRequest("account.deactivated", "Account deactivated. Contact support");

            if (rider.IsBarred)
                return Error.BadRequest("account.barred", "Account barred. Contact support");

            (accessToken, refreshToken) = await _jwtService.GenerateToken(rider);

            BackgroundJob.Enqueue(() => UpdateRiderImageUrls(rider.Id));
        }

        else if (request.AppInstance == ApplicationInstance.Driver)
        {
            var driver = await _driverRepository
                .GetByPhoneNoAsync(request.PhoneNo.ToPhoneNumber());

            if (driver is null) return new NumberVerificationResponse
            {
                NewUser = true,
                CodeVerificationStatus = true
            };

            if (driver.IsDeactivated)
                return Error.BadRequest("account.deactivated", "Account deactivated. Contact support");

            if (driver.IsBarred)
                return Error.BadRequest("account.barred", "Account barred. Contact support");

            (accessToken, refreshToken) = await _jwtService.GenerateToken(driver);

            BackgroundJob.Enqueue(() => UpdateDriverImageUrls(driver.Id));
        }

        else
            return Error.BadRequest("invalid.appinstance", "Specify a valid app instance");

        return new NumberVerificationResponse
        {
            CodeVerificationStatus = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            NewUser = false
        };
    }

    public async Task UpdateRiderImageUrls(long riderId)
    {
        var rider = await _riderRepository.GetAsync(riderId);

        if (rider is null) return;

        if(rider.ProfileImageUrlExpiry.Date <= DateTime.UtcNow.Date)
        {
            if (string.IsNullOrWhiteSpace(rider.ProfileImageUrl)) return;

            string riderProfileImageKey = UploadKeys.Rider.ProfileImage(rider.Id);

            (string imageUrl, DateTime expiry) = await _objectStoreService.GeneratePreSignedUrl(riderProfileImageKey);

            rider.UpdateProfileImageUrl(imageUrl, expiry);

            _riderRepository.Update(rider);

            await _unitOfWork.SaveChangesAsync();
        }
    }

    // todo: add to admin endpoint which returns driver/riders details...
    public async Task UpdateDriverImageUrls(long driverId)
    {
        var driver = await _driverRepository.GetAsync(driverId);

        if (driver is null) return;

        if (driver.ProfileImageUrlExpiry.HasValue && driver.ProfileImageUrlExpiry.Value.Date <= DateTime.UtcNow.Date)
        {
            string driverProfileImageKey = UploadKeys.Driver.ProfileImage(driver.Id);
            string driverLicenseImageKey = UploadKeys.Driver.DriversLicense(driver.Id);

            (string profileImageUrl, DateTime expiry) = await _objectStoreService.GeneratePreSignedUrl(driverProfileImageKey);
            (string licenseImageUrl, DateTime licenseExpiry) = await _objectStoreService.GeneratePreSignedUrl(driverLicenseImageKey);

            driver.UploadImages(profileImageUrl, licenseImageUrl, expiry);

            _driverRepository.Update(driver);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
