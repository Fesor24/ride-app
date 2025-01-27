
using Soloride.Domain.Transactions;

namespace Soloride.Domain.Models.Riders;
public sealed class RiderTransactionModel
{
    public string Reference { get; set; }
    public string Rider { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
}
