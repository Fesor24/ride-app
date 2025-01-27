using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Riders;
public sealed class PaymentCard : Entity
{
    private PaymentCard()
    {

    }

    public PaymentCard(long riderId, string authCode, string last4Digits, string bank,
        PaymentCardType paymentCardType, string expiryMonth, 
        string expiryYear, string email, string signature)
    {
        RiderId = riderId;
        AuthorizationCode = authCode;
        Last4Digits = last4Digits;
        Bank = bank;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        CardType = paymentCardType;
        Email = email;
        Signature = signature;
    }

    public long RiderId { get; private set; }
    public Rider Rider { get; }
    public string AuthorizationCode { get; private set; }
    public string Last4Digits { get; private set; }
    public string Bank { get; private set; }
    public PaymentCardType CardType { get; private set; }
    public string Email { get; private set; }
    public string Signature { get; private set; }
    public string ExpiryMonth { get; private set; }
    public string ExpiryYear { get; private set; }
}
