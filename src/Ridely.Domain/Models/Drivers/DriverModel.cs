using Ridely.Domain.Models.Common;

namespace Ridely.Domain.Models.Drivers;
public class DriverModel : BaseModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DriversLicenseNo { get; set; }
    public string DriversLicenseImageUrl { get; set; }
    public string Email { get; set; }
    public string PhoneNo { get; set; }
    public decimal AvailableBalance { get; set; }
    public string? ReferralCode { get; set; }
    public decimal AvgRatings { get; set; }
    public int CompletedRides { get; set; }
    public CabModel Cab { get; set; }
    public DateTime CreatedAt { get; set; }
}
