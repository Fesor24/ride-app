using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(nameof(Payment), ApplicationDbContext.Rides);
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Error)
            .HasColumnType("jsonb");
        builder.Property(payment => payment.Reference)
            .HasConversion<UlidToStringConverter>();
    }
}
