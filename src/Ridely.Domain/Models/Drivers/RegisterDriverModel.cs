using Soloride.Domain.Drivers;
using Soloride.Domain.Shared;

namespace Soloride.Domain.Models.Drivers;
public class RegisterDriverModel
{
    public Driver DriverInfo { get; set; } = new();
    public Vehicle DriverVehicle { get; set; } = new();
    public string? ReferrerCode { get; set; }
    public class Vehicle
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Color { get; set; }
        public string LicensePlateNo { get; set; }
        public string YearOfRelease { get; set; }
        public string Model { get; set; }
    }

    public class Driver
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string DriversLicenseNo { get; set; }
        public string DriversLicenseImageUrl { get; set; }
        public DriverService DriverService { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }
}

