using Ridely.Domain.Transactions;

namespace Ridely.Domain.Models.Payments;
public class TransactionsSearchParams : SearchParams
{
    public TransactionType? Type { get; set; }
    public int? DriverId { get; set; }
}
