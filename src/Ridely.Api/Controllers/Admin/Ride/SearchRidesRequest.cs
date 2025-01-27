using Ridely.Domain.Rides;

namespace RidelyAPI.Controllers.Admin.Ride
{
    public class SearchRidesRequest : SearchRequest
    {
        public RideStatus? RideStatus { get; set; }
        public string? DriverPhoneNo { get; set; }
        public string? RiderPhoneNo {  get; set; }  
    }
}
