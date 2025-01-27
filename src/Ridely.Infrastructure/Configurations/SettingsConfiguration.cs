using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Common;

namespace Ridely.Infrastructure.Configurations;
internal sealed class SettingsConfiguration : IEntityTypeConfiguration<Settings>
{
    public void Configure(EntityTypeBuilder<Settings> builder)
    {
        builder.ToTable(nameof(Settings), ApplicationDbContext.Common);
    }
}
