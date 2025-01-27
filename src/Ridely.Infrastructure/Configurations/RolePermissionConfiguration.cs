using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soloride.Domain.Users;

namespace Soloride.Infrastructure.Configurations;
internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable(nameof(RolePermission), ApplicationDbContext.User);
        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });
    }
}
