using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RiderReferrersConfiguration : IEntityTypeConfiguration<RiderReferrers>
{
    public void Configure(EntityTypeBuilder<RiderReferrers> builder)
    {
        builder.ToTable(nameof(RiderReferrers), ApplicationDbContext.Rider);

        builder.HasKey(refer => refer.Id);

        builder.HasOne(refer => refer.Rider)
            .WithMany(rider => rider.Referrers)
            .HasForeignKey(refer => refer.RiderId);
    }
}
