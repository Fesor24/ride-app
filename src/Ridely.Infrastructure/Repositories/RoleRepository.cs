using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Users;

namespace Soloride.Infrastructure.Repositories;
internal class RoleRepository(ApplicationDbContext context) :
    GenericRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByCodeAsync(string code) =>
        await context.Set<Role>()
        .Where(x => x.Code == code)
        .FirstOrDefaultAsync();
}
