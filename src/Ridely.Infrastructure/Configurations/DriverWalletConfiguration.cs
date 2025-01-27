using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Configurations;
internal sealed class DriverWalletConfiguration : IEntityTypeConfiguration<DriverWallet>
{
    public void Configure(EntityTypeBuilder<DriverWallet> builder)
    {
        builder.ToTable(nameof(DriverWallet), ApplicationDbContext.Driver);
        builder.HasKey(driver => driver.Id);
    }
}
