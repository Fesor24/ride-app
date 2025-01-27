using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Configurations;
internal sealed class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
{
    public void Configure(EntityTypeBuilder<BankAccount> builder)
    {
        builder.ToTable(nameof(BankAccount), ApplicationDbContext.Driver);
        builder.HasKey(account => account.Id);
        builder.Property(account => account.AccountNo)
            .HasMaxLength(20);

        builder.Property(account => account.AccountName)
            .HasMaxLength(60);

        builder.Property(account => account.RecipientCode)
            .HasMaxLength(200);
    }
}
