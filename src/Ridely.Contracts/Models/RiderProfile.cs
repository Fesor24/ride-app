namespace Ridely.Contracts.Models;
public sealed record RiderProfile(
    long Id,
    string FirstName,
    string LastName,
    string ProfileImageUrl,
    string PhoneNo
    );
