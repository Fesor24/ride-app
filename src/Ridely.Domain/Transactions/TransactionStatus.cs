namespace Ridely.Domain.Transactions;
public enum TransactionStatus
{
    Unknown = 0,
    Pending = 1,
    Success = 2,
    Failed = 3,
    Retry = 4
}

public enum TransactionType
{
    Unknown = 0,
    CardAddition = 1,
    Withdrawal = 2,
    WalletCredit = 3,
    RideCommission = 4
}
