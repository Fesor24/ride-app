namespace Ridely.Application.Abstractions.Payment;
public record PaymentResponse(long TotalRideFare, long AmountOutstanding, 
    long DiscountAmount, long WaitingTimeCharge);
