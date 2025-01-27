using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Users;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(nameof(Role), ApplicationDbContext.User);
        builder.HasKey(role => role.Id);
        builder.Property(role => role.Name)
            .HasMaxLength(20);
        builder.Property(role => role.Code)
            .HasMaxLength(20);
    }
}
