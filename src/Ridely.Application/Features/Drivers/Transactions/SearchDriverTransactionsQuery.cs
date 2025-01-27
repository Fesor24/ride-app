using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Drivers.Transactions;
public sealed class SearchDriverTransactionsQuery : IQuery<PaginatedList<SearchDriverTransactionsResponse>>
{
    public int PageSize { get; set; }
    public int PageNumber {  get; set; }
    public long DriverId {  get; set; } 
}
