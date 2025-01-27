using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Configurations;
internal sealed class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable(nameof(Driver), ApplicationDbContext.Driver);

        builder.Property(driver => driver.FirstName)
            .HasMaxLength(60);

        builder.Property(driver => driver.LastName)
            .HasMaxLength(60);

        builder.Property(driver => driver.PhoneNo)
           .HasMaxLength(15);

        builder.Property(driver => driver.Email)
            .HasMaxLength(70);

        builder.Property(driver => driver.LicenseNo)
            .HasMaxLength(40);

        builder.HasIndex(driver => driver.ReferralCode)
            .IsUnique();

        builder.Property(driver => driver.ReferralCode)
            .HasMaxLength(20);

        builder.HasIndex(driver => driver.PhoneNo);

        builder.HasIndex(driver => driver.Email);

        builder.HasQueryFilter(driver => !driver.IsDeleted);
    }
}
