using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Configurations;
internal sealed class CabConfiguration : IEntityTypeConfiguration<Cab>
{
    public void Configure(EntityTypeBuilder<Cab> builder)
    {
        builder.ToTable(nameof(Cab), ApplicationDbContext.Driver);

        builder.Property(x => x.Color)
            .HasMaxLength(20);

        builder.Property(x => x.Name)
            .HasMaxLength(30);

        builder.Property(x => x.Manufacturer)
            .HasMaxLength(30);

        builder.Property(x => x.Model)
            .HasMaxLength(50);

        builder.Property(x => x.LicensePlateNo)
          .HasMaxLength(20);

        builder.Property(x => x.Year)
            .HasMaxLength(10);
    }
}
