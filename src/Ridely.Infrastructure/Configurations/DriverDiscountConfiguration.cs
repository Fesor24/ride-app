using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Configurations;
internal sealed class DriverDiscountConfiguration : IEntityTypeConfiguration<DriverDiscount>
{
    public void Configure(EntityTypeBuilder<DriverDiscount> builder)
    {
        builder.ToTable(nameof(DriverDiscount), ApplicationDbContext.Driver);
        builder.HasKey(disc => disc.Id);
        builder.HasOne(disc => disc.Driver)
            .WithMany(driver => driver.Discounts)
            .HasForeignKey(disc => disc.DriverId);
    }
}
