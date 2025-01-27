using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Admin.Driver.Search;
public class SearchDriverQuery : IRequest<Result<PaginatedList<SearchDriverResponse>>>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public CabType? CabType { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email { get; set; }
}
