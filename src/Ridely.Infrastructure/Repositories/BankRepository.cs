using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Common;

namespace Ridely.Infrastructure.Repositories;
internal class BankRepository(ApplicationDbContext context) :
    GenericRepository<Bank>(context), IBankRepository
{
    public async Task<List<Bank>> GetAllBanks() =>
        await context.Set<Bank>()
        .OrderBy(x => x.Name)
        .ToListAsync();
}
