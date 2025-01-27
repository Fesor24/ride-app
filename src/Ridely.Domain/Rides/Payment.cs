using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Rides;
public sealed class Payment : Entity
{
    private Payment()
    {
        
    }
    public Payment(long amount, Ulid reference, PaymentMethod? paymentMethod, 
        long? paymentCardId)
    {
        Amount = amount;
        CreatedAtUtc = DateTime.UtcNow;
        Status = PaymentStatus.Pending;
        Reference = reference;
        PaymentCardId = paymentCardId;

        if(paymentMethod.HasValue)
            Method = paymentMethod.Value;
    }

    public Ulid Reference { get; private set; }
    public long Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long? PaymentCardId { get; private set; }
    public string? Error { get; private set; }

    public void UpdatePaymentMethod(PaymentMethod method, long? cardId = null)
    {
        Method = method;
        Status = PaymentStatus.Pending;
        PaymentCardId = cardId;
    }

    public void UpdateStatus(PaymentStatus status, string? error = null)
    {
        Status = status;
        Error = error;
    }

    public void UpdateAmount(long fare)
    {
        Amount = fare;
    }
}
