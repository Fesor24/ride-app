using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Drivers;
using Ridely.Domain.Shared;

namespace Ridely.Application.Features.Drivers.RegisterDriver;
public sealed record RegisterDriverCommand(
    string? ReferrerCode,
    RegisterDriverInfo PersonalInfo,
    RegisterDriverVehicleInfo VehicleInfo
    ) :
    ICommand<RegisterDriverResponse>;

public class RegisterDriverCommandValidator : AbstractValidator<RegisterDriverCommand>
{
    public RegisterDriverCommandValidator()
    {
        RuleFor(x => x.PersonalInfo.PhoneNo)
            .NotEmpty().WithMessage("Phone no can not be empty")
            .NotNull().WithMessage("Phone no can not be null")
            .MaximumLength(15).WithMessage("Phone no can not be more than 15 characters")
            .MinimumLength(10).WithMessage("Phone no must be at least 10 characters");

        RuleFor(x => x.PersonalInfo.Email)
            .NotEmpty().WithMessage("Email no can not be empty")
            .NotNull().WithMessage("Email no can not be null")
            .MaximumLength(45).WithMessage("Email can not exceed 45 characters")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.PersonalInfo.FirstName)
            .NotEmpty().WithMessage("FirstName no can not be empty")
            .NotNull().WithMessage("FirstName no can not be null");

        RuleFor(x => x.PersonalInfo.LastName)
            .NotEmpty().WithMessage("LastName no can not be empty")
            .NotNull().WithMessage("LastName no can not be null");

        RuleFor(x => x.VehicleInfo.LicensePlateNo)
            .NotEmpty().WithMessage("License plate no can not be empty")
            .NotNull().WithMessage("License plate no can not be null");

        RuleFor(x => x.VehicleInfo.Model)
            .NotEmpty().WithMessage("Vehicle model can not be empty")
            .NotNull().WithMessage("Vehicle model can not be null");

        RuleFor(x => x.VehicleInfo.Color)
            .NotEmpty().WithMessage("Vehicle color can not be empty")
            .NotNull().WithMessage("Vehicle color no can not be null");

        RuleFor(x => x.PersonalInfo.DriversLicenseNo)
            .NotEmpty().WithMessage("Drivers License no can not be empty")
            .NotNull().WithMessage("Drivers License no can not be null");
    }
}

public sealed record RegisterDriverInfo(
    string FirstName,
    string LastName,
    Gender Gender,
    string PhoneNo,
    string Email,
    string DriversLicenseNo,
    DriverService DriverService,
    string ProfileImage,
    string DriversLicense
    );

public sealed record RegisterDriverVehicleInfo(
    string Color,
    string Year,
    string Model,
    string LicensePlateNo,
    string Manufacturer,
    string Name
    );
