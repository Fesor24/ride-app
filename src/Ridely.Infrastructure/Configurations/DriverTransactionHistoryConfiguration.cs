using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class DriverTransactionHistoryConfiguration : IEntityTypeConfiguration<DriverTransactionHistory>
{
    public void Configure(EntityTypeBuilder<DriverTransactionHistory> builder)
    {
        builder.ToTable(nameof(DriverTransactionHistory), ApplicationDbContext.Driver);
        builder.HasKey(transaction => transaction.Id);
        builder.HasIndex(transaction => transaction.Reference);
        builder.Property(transaction => transaction.Reference)
            .HasConversion<UlidToStringConverter>();
        builder.Property(transaction => transaction.BankAccountDetails)
            .HasColumnType("jsonb");
    }
}
