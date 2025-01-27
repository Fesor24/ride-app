using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Transactions;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class TransactionReferenceMapConfiguration : IEntityTypeConfiguration<TransactionReferenceMap>
{
    public void Configure(EntityTypeBuilder<TransactionReferenceMap> builder)
    {
        builder.ToTable(nameof(TransactionReferenceMap), ApplicationDbContext.Transaction);
        builder.HasKey(transaction => transaction.Id);
        builder.Property(transaction => transaction.Reference)
            .HasConversion<UlidToStringConverter>();
    }
}
