using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class PaymentCardConfiguration : IEntityTypeConfiguration<PaymentCard>
{
    public void Configure(EntityTypeBuilder<PaymentCard> builder)
    {
        builder.ToTable(nameof(PaymentCard), ApplicationDbContext.Rider);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Last4Digits)
            .HasMaxLength(6);

        builder.Property(x => x.AuthorizationCode)
            .HasMaxLength(40);

        builder.Property(x => x.Bank)
            .HasMaxLength(70);

        builder.Property(x => x.ExpiryYear)
            .HasMaxLength(10);

        builder.Property(x => x.ExpiryMonth)
            .HasMaxLength(10);

        builder.HasOne(card => card.Rider)
            .WithMany(rider => rider.PaymentCards)
            .HasForeignKey(card => card.RiderId);

        builder.Property(card => card.Email)
            .HasMaxLength(200);

        builder.Property(card => card.Signature)
            .HasMaxLength(250);
    }
}
