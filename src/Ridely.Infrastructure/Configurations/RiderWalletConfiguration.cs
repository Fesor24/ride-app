using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ridely.Domain.Riders;

namespace Ridely.Infrastructure.Configurations;
internal sealed class RiderWalletConfiguration : IEntityTypeConfiguration<RiderWallet>
{
    public void Configure(EntityTypeBuilder<RiderWallet> builder)
    {
        builder.ToTable(nameof(RiderWallet), ApplicationDbContext.Rider);
        builder.HasKey(wallet => wallet.Id);
    }
}
