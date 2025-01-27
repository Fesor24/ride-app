using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RideLogConfiguration : IEntityTypeConfiguration<RideLog>
{
    public void Configure(EntityTypeBuilder<RideLog> builder)
    {
        builder.ToTable(nameof(RideLog), ApplicationDbContext.Rides);
        builder.HasKey(log => log.Id);
        builder.HasOne(log => log.Ride)
            .WithMany(ride => ride.RideLogs)
            .HasForeignKey(log => log.RideId);
    }
}
