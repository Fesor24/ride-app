using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Common;
public interface IBankRepository : IGenericRepository<Bank>
{
    Task<List<Bank>> GetAllBanks();
}
