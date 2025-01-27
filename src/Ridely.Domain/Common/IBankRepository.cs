using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Common;
public interface IBankRepository : IGenericRepository<Bank>
{
    Task<List<Bank>> GetAllBanks();
}
