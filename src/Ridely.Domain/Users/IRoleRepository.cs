using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Users;
public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByCodeAsync(string code);
}
