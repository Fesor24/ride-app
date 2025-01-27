using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Common;

namespace Soloride.Infrastructure.Repositories;
internal class BankRepository(ApplicationDbContext context) :
    GenericRepository<Bank>(context), IBankRepository
{
    public async Task<List<Bank>> GetAllBanks() =>
        await context.Set<Bank>()
        .OrderBy(x => x.Name)
        .ToListAsync();
}
