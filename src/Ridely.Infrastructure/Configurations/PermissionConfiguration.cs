using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soloride.Domain.Users;

namespace Soloride.Infrastructure.Configurations;
internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(nameof(Permission), ApplicationDbContext.User);
        builder.HasKey(permission => permission.Id);
        builder.Property(permission => permission.Name)
            .HasMaxLength(70);
        builder.Property(permission => permission.Code)
            .HasMaxLength(70);
    }
}
