using Ridely.Domain.Riders;
using Ridely.Domain.Transactions;

namespace Ridely.Api.Controllers.Admin.Rider;

public sealed class SearchRiderTransaction : SearchRequest
{
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<RiderTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
}
