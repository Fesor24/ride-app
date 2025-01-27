using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;
using Ridely.Domain.Riders;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Admin.Rider.SearchTransactions;
public sealed class SearchRiderTransactionQuery : IQuery<PaginatedList<SearchRiderTransactionResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? PhoneNo { get; set; }
    public string? Reference {  get; set; } 
    public List<RiderTransactionType> Types { get; set; } = [];
    public TransactionStatus? Status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
