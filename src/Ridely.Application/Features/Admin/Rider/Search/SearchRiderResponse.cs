namespace Soloride.Application.Features.Admin.Rider.Query.Search;
public sealed class SearchRiderResponse
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public string Gender { get; set; }
    public string CreatedAt { get; set; }
}
