namespace Soloride.Domain.Riders;
public enum RiderTransactionType
{
    FundWallet = 1,
    CardAddition = 2,
    RidePaymentFromVirtualWallet = 3,
    RidePaymentUsingPaystack = 4,
    Refund = 5
}
