using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Users;
public sealed class Permission : Entity
{
    private Permission()
    {
        
    }

    public Permission(string name, string code)
    {
        Name = name;
        Code = code;
    }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public ICollection<RolePermission> RolePermissions { get; } = [];
}
