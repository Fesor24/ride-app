using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soloride.Domain.Drivers;

namespace Soloride.Infrastructure.Configurations;
internal sealed class DriverReferrersConfiguration : IEntityTypeConfiguration<DriverReferrers>
{
    public void Configure(EntityTypeBuilder<DriverReferrers> builder)
    {
        builder.ToTable(nameof(DriverReferrers), ApplicationDbContext.Driver);

        builder.HasKey(refer => refer.Id);

        builder.HasOne(refer => refer.Driver)
            .WithMany(driver => driver.DriverReferrers)
            .HasForeignKey(refer => refer.DriverId);
    }
}
