namespace Ridely.Application.Features.Admin.Users.Query.GetProfile;
public class GetProfileResponse
{
    public long Id { get; set; }
    public string Role { get; set; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
