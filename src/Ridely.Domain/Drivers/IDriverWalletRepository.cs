using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Drivers;
public interface IDriverWalletRepository : IGenericRepository<DriverWallet>
{
    Task<DriverWallet?> GetByDriverAsync(long driverId);
}
