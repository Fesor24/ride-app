namespace RidelyAPI.Controllers.Admin.Rider
{
    public class SearchRiderRequest : SearchRequest
    {
        public string? Email { get; set; }
        public string? PhoneNo { get; set; }
    }
}
