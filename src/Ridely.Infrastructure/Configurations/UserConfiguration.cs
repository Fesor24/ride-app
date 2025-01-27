using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Users;

namespace Ridely.Infrastructure.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User), ApplicationDbContext.User);

        builder.HasIndex(x => x.PhoneNo);

        builder.HasIndex(x => x.Email);

        builder.Property(x => x.PhoneNo)
            .HasMaxLength(15);

        builder.Property(x => x.Email)
            .HasMaxLength(50);

        builder.Property(x => x.FirstName)
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasMaxLength(50);

        builder.HasQueryFilter(user => !user.IsDeleted);
    }
}
