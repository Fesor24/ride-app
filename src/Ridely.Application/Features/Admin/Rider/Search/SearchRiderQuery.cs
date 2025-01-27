using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Admin.Rider.Query.Search;
public sealed class SearchRiderQuery : IQuery<PaginatedList<SearchRiderResponse>>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email { get; set; }
}
