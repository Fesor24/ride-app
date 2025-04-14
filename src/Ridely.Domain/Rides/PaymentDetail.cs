using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public sealed class PaymentDetail : Entity
{
    private PaymentDetail()
    {
        
    }

    public PaymentDetail(Ulid reference, long paymentId, PaymentFor paymentFor, long amount)
    {
        Reference = reference;
        PaymentId = paymentId;
        PaymentFor = paymentFor;
        Amount = amount;
        AmountDue = amount;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Ulid Reference { get; private set; }
    public long PaymentId { get; private set; }
    public Payment Payment { get; private set; }
    public PaymentFor PaymentFor { get; private set; }
    public long Amount { get; private set; }
    public long AmountDue {  get; private set; }
    public long Credit { get; private set; }
    public int Sequence { get; private set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? Error { get; private set; }
    public DateTime PaidAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void UpdateStatus(PaymentStatus paymentStatus, string? error = null)
    {
        PaymentStatus = paymentStatus;
        Error = error;

        if(paymentStatus == PaymentStatus.Success)
        {
            AmountDue = 0;
            PaidAtUtc = DateTime.UtcNow;
        }
            
        else
            UpdatedAtUtc = DateTime.UtcNow;
    }

    public void IncreaseOrOverrideAmountDue(long amount, bool overrideCurrent = false)
    {
        if (overrideCurrent) AmountDue = amount;
        else
        {
            AmountDue += amount;
            Amount += amount;
        }
    }

    public void DecreaseAmountDue(long amount)
    {
        AmountDue -= amount;
    }

    public void UpdateCreditAmount(long amount)
    {
        Credit += amount;
    }
}
