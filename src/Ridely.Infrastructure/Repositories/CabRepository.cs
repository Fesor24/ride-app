using Soloride.Domain.Drivers;

namespace Soloride.Infrastructure.Repositories;
internal sealed class CabRepository(ApplicationDbContext context) : 
    GenericRepository<Cab>(context), ICabRepository
{
}
