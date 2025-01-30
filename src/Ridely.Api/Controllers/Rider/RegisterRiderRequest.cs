using Ridely.Domain.Shared;

namespace Ridely.Api.Controllers.Rider;

public sealed record RegisterRiderRequest(
    string? ReferrerCode,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNo,
    Gender Gender
    );
