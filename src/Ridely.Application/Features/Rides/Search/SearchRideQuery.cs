using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.Search;
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
