using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RiderDiscountConfiguration : IEntityTypeConfiguration<RiderDiscount>
{
    public void Configure(EntityTypeBuilder<RiderDiscount> builder)
    {
        builder.ToTable(nameof(RiderDiscount), ApplicationDbContext.Rider);
        builder.HasKey(disc => disc.Id);
        builder.HasOne(disc => disc.Rider)
            .WithMany(rider => rider.Discounts)
            .HasForeignKey(disc => disc.RiderId);
    }
}
