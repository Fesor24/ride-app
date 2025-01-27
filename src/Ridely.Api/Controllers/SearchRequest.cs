namespace RidelyAPI.Controllers
{
    public abstract class SearchRequest
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
