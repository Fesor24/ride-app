using Ridely.Domain.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class CabRepository(ApplicationDbContext context) : 
    GenericRepository<Cab>(context), ICabRepository
{
}
