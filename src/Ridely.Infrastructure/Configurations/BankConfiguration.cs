using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soloride.Domain.Common;

namespace Soloride.Infrastructure.Configurations;
internal sealed class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.ToTable(nameof(Bank), ApplicationDbContext.Common);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Code)
          .HasMaxLength(40);
        builder.Property(bank => bank.Type)
            .HasMaxLength(150);
    }
}
