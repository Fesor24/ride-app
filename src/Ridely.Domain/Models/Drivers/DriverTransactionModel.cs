using Soloride.Domain.Drivers;
using Soloride.Domain.Transactions;

namespace Soloride.Domain.Models.Drivers;
public sealed class DriverTransactionModel
{
    public string Reference { get; set; }
    public string Driver { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DriverTransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
