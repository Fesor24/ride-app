using Ridely.Domain.Drivers;
using Ridely.Domain.Transactions;

namespace Ridely.Api.Controllers.Admin.Driver;

public class SearchDriverTransactionRequest : SearchRequest
{
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<DriverTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
}
