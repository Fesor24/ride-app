using Ridely.Domain.Drivers;
using Ridely.Domain.Shared;

namespace RidelyAPI.Controllers.Driver;

public sealed record RegisterDriverRequest(
    string? ReferrerCode,
    PersonalInfo Driver,
    VehicleInfo Vehicle
    );

public sealed record PersonalInfo(
    string FirstName,
    string LastName,
    Gender Gender,
    string PhoneNo,
    string Email,
    string DriversLicenseNo,
    DriverService DriverService,
    string ProfileImageBase64Url,
    string DriversLicenseBase64Url,
    string IdentityNo,
    IdentityType IdentityType
    );

public sealed record VehicleInfo(
    string Color,
    string Year,
    string Model,
    string LicensePlateNo,
    string Manufacturer
    );
