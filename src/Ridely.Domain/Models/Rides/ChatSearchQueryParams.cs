namespace Ridely.Domain.Models.Rides;
public class ChatSearchQueryParams : SearchParams
{
    public long RideId { get; set; }
    public long? RiderId { get; set; }
    public long? DriverId { get; set;}
}
