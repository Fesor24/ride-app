namespace Ridely.Contracts.Models;
public sealed record DriverProfile(
    long Id,
    string FirstName,
    string LastName,
    string DeviceTokenId,
    string ProfileImageUrl,
    decimal Ratings);
