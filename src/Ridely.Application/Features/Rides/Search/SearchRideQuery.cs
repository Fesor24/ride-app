using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Rides.Search;
public sealed class SearchRideQuery : IQuery<PaginatedList<SearchRideResponse>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long? DriverId { get; set; }
    public long? RiderId { get; set; }
    public List<RideStatus> RideStatus { get; set; } = [];
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
