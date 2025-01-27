using Soloride.Domain.Shared;

namespace SolorideAPI.Controllers.Rider;

public sealed record RegisterRiderRequest(
    string? ReferrerCode,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNo,
    Gender Gender
    );
