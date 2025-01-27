namespace Soloride.Application.Features.Admin.Driver.Search;
public class SearchDriverResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public int CompletedRides { get; set; }
    public decimal AvgRatings { get; set; }
    public string CabType { get; set; }
    public DateTime CreatedAt { get; set; }
}
