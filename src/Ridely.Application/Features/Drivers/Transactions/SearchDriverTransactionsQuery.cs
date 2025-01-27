using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;

namespace Ridely.Application.Features.Drivers.Transactions;
public sealed class SearchDriverTransactionsQuery : IQuery<PaginatedList<SearchDriverTransactionsResponse>>
{
    public int PageSize { get; set; }
    public int PageNumber {  get; set; }
    public long DriverId {  get; set; } 
}
