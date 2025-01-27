using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Riders;
public interface IRiderWalletRepository : IGenericRepository<RiderWallet>
{
    Task<RiderWallet?> GetByRiderAsync(long riderId);
}
