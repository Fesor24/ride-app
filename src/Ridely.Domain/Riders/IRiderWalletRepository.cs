using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Riders;
public interface IRiderWalletRepository : IGenericRepository<RiderWallet>
{
    Task<RiderWallet?> GetByRiderAsync(long riderId);
}
