using Ridely.Domain.Drivers;
using Ridely.Domain.Transactions;

namespace Ridely.Domain.Models.Drivers;
public sealed class DriverTransactionSearchParams : SearchParams
{
    public long? DriverId { get; set; }
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<DriverTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
}
