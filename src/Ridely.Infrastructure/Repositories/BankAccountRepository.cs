using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class BankAccountRepository(ApplicationDbContext context) :
    GenericRepository<BankAccount>(context), IBankAccountRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<List<BankAccount>> GetAllByDriverAsync(long driverId) =>
       await _context.Set<BankAccount>()
       .Where(x => x.DriverId == driverId)
       .Include(x => x.Bank)
       .ToListAsync();

    public async Task<BankAccount?> GetByDriverAsync(long driverId, long bankAccountId) =>
       await _context.Set<BankAccount>()
       .Where(x => x.DriverId == driverId && x.Id == bankAccountId)
       .Include(x => x.Bank)
       .FirstOrDefaultAsync();
}
