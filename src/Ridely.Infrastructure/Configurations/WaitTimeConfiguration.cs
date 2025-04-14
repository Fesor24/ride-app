using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Configurations;
internal sealed class WaitTimeConfiguration : IEntityTypeConfiguration<WaitTime>
{
    public void Configure(EntityTypeBuilder<WaitTime> builder)
    {
        builder.ToTable(nameof(WaitTime), ApplicationDbContext.Rides);
        builder.HasKey(wt => wt.Id);
    }
}
