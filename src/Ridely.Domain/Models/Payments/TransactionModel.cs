using Ridely.Domain.Transactions;

namespace Ridely.Domain.Models.Payments;
public class TransactionModel : BaseModel
{
    public string Reference { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionType Type { get; set; }
}
