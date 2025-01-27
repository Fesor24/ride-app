using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Drivers;
using Soloride.Domain.Models;
using Soloride.Domain.Transactions;

namespace Soloride.Application.Features.Admin.Driver.SearchTransactions;
public sealed class SearchDriverTransactionQuery : IQuery<PaginatedList<SearchDriverTransactionResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? PhoneNo { get; set; }
    public string? Reference { get; set; }
    public List<DriverTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
