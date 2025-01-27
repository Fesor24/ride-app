using Ridely.Domain.Abstractions;
using Ridely.Domain.Shared;

namespace Ridely.Domain.Riders;
public sealed class Rider : Entity
{
    private Rider()
    {
        
    }

    private Rider(string firstName, string lastName, string phoneNo, string email,
        Gender gender, DateOnly? dateOfBirth = null)
    {
        if (firstName.Length > 60)
            throw new ApplicationException("First name: Maximum character (60) exceeded");
        if (lastName.Length > 60)
            throw new ApplicationException("Last name: Maximum character (60) exceeded");
        if (phoneNo.Length > 15)
            throw new ApplicationException("Phone no: Maximum character (15) exceeded");
        if (email.Length > 70)
            throw new ApplicationException("Email: Maximum character (70) exceeded");

        FirstName = firstName;
        LastName = lastName;
        PhoneNo = phoneNo;
        Email = email;
        Gender = gender;
        Status = RiderStatus.Online;
        CreatedAtUtc = DateTime.UtcNow;
        DateOfBirth = dateOfBirth;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PhoneNo { get; private set; }
    public string Email { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public string ProfileImageUrl { get; private set; } = "";
    public DateTime ProfileImageUrlExpiry { get; private set; } 
    public string? ReferralCode { get; private set; }
    public long? ReferredByUserId { get; private set; }
    public ReferredUser ReferredByUser { get; private set; }
    public RiderStatus Status { get; private set; }
    public double Lat { get; set; }
    public double Long { get; set; }
    public string? DeviceTokenId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public string? RefreshToken { get; private set; }
    public bool IsDeactivated { get; private set; } = false;
    public bool IsDeleted { get; private set; } = false;
    public bool IsBarred { get; private set; } = false;
    public DateTime? RefreshTokenExpiry { get; private set; }
    public long? CurrentRideId { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public ICollection<PaymentCard> PaymentCards { get; private set; } = [];
    public ICollection<SavedLocation> SavedLocations { get; private set; } = [];
    public ICollection<RiderReferrers> Referrers { get; private set; } = [];

    public void UpdateRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);
    }

    public void UpdateStatus(RiderStatus status, long? rideId = null)
    {
        Status = status;
        CurrentRideId = rideId;
    }

    public void Deactivate()
    {
        IsDeactivated = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Delete()
    {
        IsDeleted = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateDeviceToken(string token)
    {
        DeviceTokenId = token;
    }

    public void UpdateReferredBy(long referredByUserId, ReferredUser referredUser)
    {
        ReferredByUserId = referredByUserId;
        ReferredByUser = referredUser;
    }

    public void UpdateReferralCode(string referralCode)
    {
        ReferralCode = referralCode.ToLowerInvariant();
    }

    public void UpdateProfileImageUrl(string profileImageUrl, DateTime expiry)
    {
        ProfileImageUrl = profileImageUrl;
        ProfileImageUrlExpiry = expiry;
    }

    public static Rider Create(string firstName, string lastName,
        string email, string phoneNo, Gender gender, 
        string referredByCode, DateOnly? dateOfBirth = null)
    {
        Rider rider = new(
            firstName, lastName,
            phoneNo, email,
            gender, dateOfBirth
            );

        return rider;
    }
}
