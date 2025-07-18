﻿using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Users;
public sealed class RolePermission : Entity
{
    public long RoleId { get; private set; }
    public Role Role { get; }
    public long PermissionId { get; private set; }
    public Permission Permission { get; }
}
