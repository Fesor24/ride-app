using Hangfire;
using MediatR;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Drivers.Events;
using DriverDomain = Ridely.Domain.Drivers.Driver;

namespace Ridely.Application.Features.Drivers.RegisterDriver;
internal sealed class RegisterDriverCommandHandler :
    ICommandHandler<RegisterDriverCommand, RegisterDriverResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _tokenService;
    private readonly IDriverRepository _driverRepository;
    private readonly ICabRepository _cabRepository;
    private readonly IDriverWalletRepository _driverWalletRepository;
    private readonly IPublisher _publisher;

    public RegisterDriverCommandHandler(IUnitOfWork unitOfWork,
        IJwtService tokenService, IDriverRepository driverRepository,
        ICabRepository cabRepository, IDriverWalletRepository driverWalletRepository,
        IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _driverRepository = driverRepository;
        _cabRepository = cabRepository;
        _driverWalletRepository = driverWalletRepository;
        _publisher = publisher;
    }

    public async Task<Result<RegisterDriverResponse>> Handle(RegisterDriverCommand request,
        CancellationToken cancellationToken)
    {
        string phoneNo = request.PersonalInfo.PhoneNo.ToPhoneNumber();

        bool driverWithPhoneNoExist = await _driverRepository
            .GetByPhoneNoAsync(phoneNo) != null;

        if (driverWithPhoneNoExist) return Error.BadRequest("phoneno.exist", "User with phone no exist");

        string email = request.PersonalInfo.Email
            .Replace(" ", "")
            .Trim()
            .ToLowerInvariant();

        bool driverWithEmailExist = await _driverRepository
            .GetByEmailAsync(email) != null;

        if (driverWithEmailExist) return Error.BadRequest("email.exist", "User with email exist");

        Cab cab = new(
            request.VehicleInfo.Name,
            request.VehicleInfo.Manufacturer,
            request.VehicleInfo.Color,
            request.VehicleInfo.Model,
            request.VehicleInfo.LicensePlateNo,
            request.VehicleInfo.Year);

        await _cabRepository.AddAsync(cab);

        await _unitOfWork.SaveChangesAsync();

        DriverDomain driver = DriverDomain.Create(
            request.PersonalInfo.FirstName,
            request.PersonalInfo.LastName,
            email,
            phoneNo,
            request.PersonalInfo.Gender,
            request.PersonalInfo.DriversLicenseNo,
            request.PersonalInfo.DriverService,
            cab.Id, request.PersonalInfo.IdentityType, request.PersonalInfo.IdentityNo
            );

        await _driverRepository.AddAsync(driver);

        //driver.RaiseDomainEvent(new DriverRegisteredDomainEvent(
        //    phoneNo,
        //    request.ReferrerCode ?? "",
        //    request.PersonalInfo.ProfileImage,
        //    request.PersonalInfo.DriversLicense));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        DriverWallet wallet = new(driver.Id);

        await _driverWalletRepository.AddAsync(wallet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        (string accessToken, string refreshToken) = await _tokenService
            .GenerateToken(driver);

        BackgroundJob.Enqueue(() => _publisher.Publish(new DriverRegisteredDomainEvent(
            phoneNo,
            request.ReferrerCode ?? "",
            request.PersonalInfo.ProfileImage,
            request.PersonalInfo.DriversLicense), cancellationToken));

        return new RegisterDriverResponse(accessToken, refreshToken);
    }
}
