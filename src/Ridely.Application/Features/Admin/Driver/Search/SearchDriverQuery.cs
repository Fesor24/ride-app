using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;

namespace Ridely.Application.Features.Admin.Driver.Search;
public class SearchDriverQuery : IRequest<Result<PaginatedList<SearchDriverResponse>>>
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public CabType? CabType { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email { get; set; }
}
