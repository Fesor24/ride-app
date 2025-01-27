using Ridely.Domain.Riders;

namespace Ridely.Domain.Models.Riders;
public class RiderModel : BaseModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public RiderStatus Status { get; set; }
    public string Email { get; set; }
    public string PhoneNo { get; set; }
    public decimal AvailableBalance { get; set; }
    public string? ReferralCode { get; set; }
    public List<CardModel> Cards { get; set; } = [];
    public string? DeviceTokenId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ProfileImageUrl { get; set; }
}

public class CardModel : BaseModel
{
    public string CardType { get; set; }
    public string Last4Digits { get; set; }
    public string ExpiryMonth { get; set; }
    public string ExpiryYear { get; set; }
}
