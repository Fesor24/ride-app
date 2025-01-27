using Soloride.Domain.Riders;
using Soloride.Domain.Transactions;

namespace Soloride.Domain.Models.Riders;
public sealed class RiderTransactionSearchParams : SearchParams
{
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<RiderTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
}
