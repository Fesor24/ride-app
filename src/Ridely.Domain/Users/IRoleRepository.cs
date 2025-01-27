using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Users;
public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByCodeAsync(string code);
}
