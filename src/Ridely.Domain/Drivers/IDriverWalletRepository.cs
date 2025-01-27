using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public interface IDriverWalletRepository : IGenericRepository<DriverWallet>
{
    Task<DriverWallet?> GetByDriverAsync(long driverId);
}
