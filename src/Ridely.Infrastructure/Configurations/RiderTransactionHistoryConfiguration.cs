using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RiderTransactionHistoryConfiguration : IEntityTypeConfiguration<RiderTransactionHistory>
{
    public void Configure(EntityTypeBuilder<RiderTransactionHistory> builder)
    {
        builder.ToTable(nameof(RiderTransactionHistory), ApplicationDbContext.Rider);
        builder.HasKey(transaction => transaction.Id);
        builder.HasIndex(transaction => transaction.Reference);
        builder.Property(transaction => transaction.Reference)
            .HasConversion<UlidToStringConverter>();
    }
}
