using Ridely.Domain.Shared;

namespace Ridely.Domain.Models.Riders;
public class RegisterRiderModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Gender Gender { get; set; }
    public string? ReferrerCode { get; set; }
}
