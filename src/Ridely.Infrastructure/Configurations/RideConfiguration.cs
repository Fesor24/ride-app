using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Configurations;

internal sealed class RideConfiguration : IEntityTypeConfiguration<Ride>
{
    public void Configure(EntityTypeBuilder<Ride> builder)
    {
        builder.ToTable(nameof(Ride), ApplicationDbContext.Rides);
        builder.HasKey(ride => ride.Id);
        builder.Property<uint>("Version")
            .IsRowVersion();
    }
}
