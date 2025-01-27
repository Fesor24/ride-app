using Ridely.Domain.Rides;

namespace Ridely.Domain.Models.Rides;
public sealed class RideSearchParams: SearchParams
{
    public long? DriverId { get; set; }
    public long? RiderId { get; set; }
    public string? DriverPhoneNo { get; set; }
    public string? RiderPhoneNo { get; set; } 
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public List<RideStatus> RideStatus { get; set; } = [];
}
