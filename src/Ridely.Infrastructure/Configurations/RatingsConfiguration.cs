using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RatingsConfiguration : IEntityTypeConfiguration<Ratings>
{
    public void Configure(EntityTypeBuilder<Ratings> builder)
    {
        builder.ToTable(nameof(Ratings), ApplicationDbContext.Rides);
        builder.HasKey(rating => rating.Id);
        builder.Property(rating => rating.Feedback)
            .HasMaxLength(120);
    }
}
