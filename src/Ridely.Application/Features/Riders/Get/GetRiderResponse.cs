using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.Get;
public class GetRiderResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNo { get; set; }
    public string? DeviceTokenId { get; set; }
    public RiderStatus Status { get; set; }
    public decimal AvailableBalance { get; set; }
    public string ProfileImage { get; set; }
    public List<GetCardResponse> Cards { get; set; } = [];
    public GetReferralInfo ReferralInfo { get; set; } = new();
}

public class GetCardResponse
{
    public int Id { get; set; }
    public string CardType { get; set; }
    public string Last4Digits { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
}

public class GetReferralInfo
{
    public string? ReferralCode { get; set; }
    public int DriversReferred { get; set; }
    public int RidersReferred { get; set; }
}
