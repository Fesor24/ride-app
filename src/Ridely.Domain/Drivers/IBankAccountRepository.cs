using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Drivers;
public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<List<BankAccount>> GetAllByDriverAsync(long driverId);
    Task<BankAccount?> GetByDriverAsync(long driverId, long bankAccountId);
}
