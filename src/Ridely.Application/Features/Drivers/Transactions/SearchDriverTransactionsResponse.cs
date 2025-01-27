using Soloride.Domain.Drivers;
using Soloride.Domain.Transactions;

namespace Soloride.Application.Features.Drivers.Transactions;
public sealed class SearchDriverTransactionsResponse
{
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public DriverTransactionType Type { get; set; }
}
