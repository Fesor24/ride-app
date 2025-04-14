using System.Text.Json;
using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public sealed class Payment : Entity
{
    private Payment()
    {
        
    }
    public Payment(PaymentMethod paymentMethod, long? paymentCardId = null)
    {
        CreatedAtUtc = DateTime.UtcNow;
        Status = PaymentStatus.Pending;
        PaymentCardId = paymentCardId;
        Method = paymentMethod;
    }

    //public Ulid Reference { get; private set; }
    // not necessarily amount paid...
    // the accumulative amount...without discount...
    public long Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public long? PaymentCardId { get; private set; }
    public string? Error { get; private set; }
    public int DiscountInPercent { get; private set; }
    public ICollection<PaymentDetail> Details { get; private set; } = [];
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

    public void UpdateAmount(long amount)
    {
        Amount = amount;
    }
}
