using Ridely.Application.Features.Riders.Get;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Drivers.Get;
public class GetDriverResponse
{
    // NOTE: Used by both admin and mobile
    public DriverResponse Driver { get; set; }
    public CabResponse Cab { get; set; }
    public GetReferralInfo ReferralInfo { get; set; } = new();
}

public class DriverResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DriverStatus Status { get; set; }
    public string LicenseNo { get; set; }
    public string ProfileImageUrl { get; set; }
    public string LicenseImageUrl { get; set; }
    public decimal AvailableBalance { get; set; }
    public DateTime ZeroCommissionTripsExpiry { get; set; }
    public string? DeviceTokenId { get; set; }
    public decimal AvgRatings { get; set; }
    public int CompletedRides { get; set; }
    public bool IdentityValidated { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
}

public class CabResponse
{
    public string Name { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Year { get; set; }
    public string LicensePlateNo { get; set; }
    public string Color { get; set; }
}
