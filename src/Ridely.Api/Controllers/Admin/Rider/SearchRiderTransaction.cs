using Soloride.Domain.Riders;
using Soloride.Domain.Transactions;

namespace SolorideAPI.Controllers.Admin.Rider;

public sealed class SearchRiderTransaction : SearchRequest
{
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<RiderTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
}
