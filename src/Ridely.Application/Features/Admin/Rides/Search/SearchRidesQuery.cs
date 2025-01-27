using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Admin.Rides.Search;
public sealed class SearchRidesQuery : IQuery<PaginatedList<SearchRidesResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public List<RideStatus> Status { get; set; } = [];
    public DateTime? From { get; set; } 
    public DateTime? To { get; set; }
    public string? DriverPhoneNo { get; set; }
    public string? RiderPhoneNo { get; set; }
}
