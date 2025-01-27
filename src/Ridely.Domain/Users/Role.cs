using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Users;
public sealed class Role : Entity
{
    private Role()
    {
        
    }

    public Role(string name, string code)
    {
        Name = name;
        Code = code;
    }
    public string Name { get; set; }
    public string Code { get; set; }
    public ICollection<RolePermission> RolePermissions { get; } = [];
}
