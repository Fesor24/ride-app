using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Rides;
using Ridely.Infrastructure.Converter;

namespace Ridely.Infrastructure.Configurations;
internal sealed class PaymentDetailConfiguration : IEntityTypeConfiguration<PaymentDetail>
{
    public void Configure(EntityTypeBuilder<PaymentDetail> builder)
    {
        builder.ToTable(nameof(PaymentDetail), ApplicationDbContext.Rides);
        builder.HasKey(detail => detail.Id);
        builder.Property(detail => detail.Reference)
            .HasConversion<UlidToStringConverter>();
        builder.HasOne(detail => detail.Payment)
            .WithMany(payment => payment.Details)
            .HasForeignKey(detail => detail.PaymentId);
    }
}
