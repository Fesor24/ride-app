using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Soloride.Domain.Riders;

namespace Soloride.Infrastructure.Configurations;
internal sealed class RiderWalletConfiguration : IEntityTypeConfiguration<RiderWallet>
{
    public void Configure(EntityTypeBuilder<RiderWallet> builder)
    {
        builder.ToTable(nameof(RiderWallet), ApplicationDbContext.Rider);
        builder.HasKey(wallet => wallet.Id);
    }
}
