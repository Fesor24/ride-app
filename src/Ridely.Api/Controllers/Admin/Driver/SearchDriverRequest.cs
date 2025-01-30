namespace Ridely.Api.Controllers.Admin.Driver;

public class SearchDriverRequest
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNo { get; set; }
}
