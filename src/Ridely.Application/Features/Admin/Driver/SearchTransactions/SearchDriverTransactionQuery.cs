using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Admin.Driver.SearchTransactions;
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
