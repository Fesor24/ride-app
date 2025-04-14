using System.ComponentModel.DataAnnotations.Schema;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Shared;

namespace Ridely.Domain.Drivers;
public sealed class Driver : Entity
{
    private Driver()
    {
    }

    private Driver(string firstName, string lastName, string email,
        string phoneNo, Gender gender, string licenseNo,
        DriverService driverService, long cabId, IdentityType identityType, string identityNo)
    {
        if (firstName.Length > 60)
            throw new ApplicationException("First name: Maximum character (60) exceeded");
        if (lastName.Length > 60)
            throw new ApplicationException("Last name: Maximum character (60) exceeded");
        if (phoneNo.Length > 15)
            throw new ApplicationException("Phone no: Maximum character (15) exceeded");
        if (email.Length > 70)
            throw new ApplicationException("Email: Maximum character (70) exceeded");
        if (licenseNo.Length > 40)
            throw new ApplicationException("License No: Maximum character (40) exceeded");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNo = phoneNo;
        Gender = gender;
        LicenseNo = licenseNo;
        DriverService = driverService;
        CreatedAtUtc = DateTime.UtcNow;
        CabId = cabId;
        IdentityTypeImageUrl = string.Empty;
        IdentityNo = identityNo;
        IdentityType = identityType;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNo { get; private set; }
    public string ProfileImageUrl { get; private set; } = string.Empty;
    public DateTime? ProfileImageUrlExpiry { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public bool IdentityValidated { get; private set; } = false;
    public string LicenseNo { get; private set; }
    public string LicenseImageUrl { get; private set; } = string.Empty;
    public IdentityType IdentityType { get; private set; }
    public string IdentityTypeImageUrl { get; private set; }
    public string IdentityNo { get; private set; }
    public DateTime? LicenseImageUrlExpiry { get; private set; }
    public double Lat { get; private set; }
    public double Long { get; private set; }
    public DriverStatus Status { get; private set; } = DriverStatus.Online;
    public long CabId { get; private set; }

    [ForeignKey(nameof(CabId))]
    public Cab Cab { get; private set; }
    public int CompletedTrips { get; private set; }
    public int RidesRated { get; private set; }
    public decimal AvgRatings { get; private set; }
    public int RideRequestsDeclined { get; private set; }
    public DriverService DriverService { get; private set; }
    public string? ReferralCode { get; private set; }
    public long? ReferredByUserId { get; private set; }
    public ReferredUser ReferredByUser { get; private set; }
    public string? DeviceTokenId { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiry { get; private set; }
    public bool IsDeactivated { get; private set; } = false;
    public bool IsDeleted { get; private set; } = false;
    public bool IsBarred { get; private set; } = false;
    public bool EmailValidated { get; private set; } = false;
    public long? CurrentRideId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public ICollection<BankAccount> BankAccounts { get; private set; } = [];
    public ICollection<DriverReferrers> DriverReferrers { get; private set; } = [];
    public ICollection<DriverDiscount> Discounts { get; private set; } = [];

    public void SetStatusAndUpdateLocation(double lat, double longitude, DriverStatus status)
    {
        Status = status;
        Long = longitude;
        Lat = lat;
    }

    public void UpdateRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = DateTime.UtcNow.AddMonths(3);
    }

    public void UpdateDeclinedRides()
    {
        RideRequestsDeclined++;
    }

    public void UpdateStatus(DriverStatus status, long? rideId = null)
    {
        Status = status;
        CurrentRideId = rideId;
    }

    public void UpdateCompletedTrips()
    {
        CompletedTrips++;
    }

    public void UpdateRatings(int rating)
    {
        var ratings = RidesRated * AvgRatings;

        ratings += rating;

        var avgRating = ratings / (RidesRated + 1);

        AvgRatings = avgRating;
        RidesRated++;
    }

    public void ValidateIdentity()
    {
        IdentityValidated = true;
    }

    public void ValidateEmail()
    {
        EmailValidated = true;
    }

    public void Deactivate()
    {
        IsDeactivated = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Delete()
    {
        Email = Email + "_deleted_" + DateTime.UtcNow.ToString();
        PhoneNo = PhoneNo + "_deleted_" + DateTime.UtcNow.ToString();
        IsDeleted = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDeviceToken(string token)
    {
        DeviceTokenId = token;
    }

    public void UploadImages(string profileImageUrl, string licenseImageUrl, DateTime expiry)
    {
        if (!string.IsNullOrWhiteSpace(profileImageUrl))
        {
            ProfileImageUrl = profileImageUrl;
            ProfileImageUrlExpiry = expiry;
        }

        if (!string.IsNullOrWhiteSpace(licenseImageUrl))
        {
            LicenseImageUrl = licenseImageUrl;
            LicenseImageUrlExpiry = expiry;
        }
    }

    public void UpdateReferralCode(string referralCode)
    {
        ReferralCode = referralCode.ToLowerInvariant();
    }

    public void UpdateReferredBy(long referredByUserId, ReferredUser referredUser)
    {
        ReferredByUserId = referredByUserId;
        ReferredByUser = referredUser;
    }

    public static Driver Create(string firstName, string lastName,
        string email, string phoneNo, Gender gender,
        string licenseNo, DriverService service, long cabId, IdentityType identityType, string identityNo)
    {
        Driver driver = new (firstName, lastName, email, phoneNo, gender,
            licenseNo, service, cabId, identityType, identityNo);

        return driver;
    }
}
