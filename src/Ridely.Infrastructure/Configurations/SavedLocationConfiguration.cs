using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class SavedLocationConfiguration : IEntityTypeConfiguration<SavedLocation>
{
    public void Configure(EntityTypeBuilder<SavedLocation> builder)
    {
        builder.ToTable(nameof(SavedLocation), ApplicationDbContext.Rider);

        builder.HasKey(location => location.Id);

        builder.Property(location => location.Address)
            .HasMaxLength(200);
    }
}
