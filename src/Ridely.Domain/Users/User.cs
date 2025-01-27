using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Users;
public sealed class User : AuditableEntity
{
    private User()
    {
        
    }

    public User(string firstName, string lastName, 
        string email, string phoneNo, long roleId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNo = phoneNo;
        RoleId = roleId;
        Status = UserStatus.Active;
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; } 
    public string PhoneNo { get; set; }
    public string? Password { get; set; }
    public UserStatus Status { get; set; }
    public long RoleId { get; set; }
    public Role Role { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsDeleted { get; private set; } = false;
    public DateTime? RefreshTokenExpiry { get; set; }
}
