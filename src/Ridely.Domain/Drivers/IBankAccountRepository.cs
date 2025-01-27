using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public interface IBankAccountRepository : IGenericRepository<BankAccount>
{
    Task<List<BankAccount>> GetAllByDriverAsync(long driverId);
    Task<BankAccount?> GetByDriverAsync(long driverId, long bankAccountId);
}
