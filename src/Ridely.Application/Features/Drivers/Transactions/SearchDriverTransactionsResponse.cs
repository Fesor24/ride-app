using Ridely.Domain.Drivers;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Drivers.Transactions;
public sealed class SearchDriverTransactionsResponse
{
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public DriverTransactionType Type { get; set; }
}
