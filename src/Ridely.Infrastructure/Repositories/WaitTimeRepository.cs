using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Repositories;
internal sealed class WaitTimeRepository(ApplicationDbContext context) : 
    GenericRepository<WaitTime>(context), IWaitTimeRepository
{
    private readonly ApplicationDbContext _context = context;
}
