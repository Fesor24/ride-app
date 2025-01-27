namespace Ridely.Domain.Transactions;
public enum TransactionLogEvent
{
    PaystackGeneralEvent = 1,
    PaystackWithdrawalEvent = 2,
    PaystackChargeEvent = 3,
    PaystackRefundEvent = 4
}
