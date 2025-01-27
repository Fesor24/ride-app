using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Transactions;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class TransactionLogConfiguration : IEntityTypeConfiguration<TransactionLog>
{
    public void Configure(EntityTypeBuilder<TransactionLog> builder)
    {
        builder.ToTable(nameof(TransactionLog), ApplicationDbContext.Transaction);
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Content)
            .HasColumnType("jsonb");
        builder.Property(log => log.Event)
            .HasConversion(type => type.ToString(),
            stringValue => (TransactionLogEvent)Enum.Parse(typeof(TransactionLogEvent), stringValue));
        builder.Property(log => log.Reference)
            .HasConversion<UlidToStringConverter>();
    }
}
