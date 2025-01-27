using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Models.Common;
using Soloride.Domain.Riders;

namespace Soloride.Infrastructure.Repositories;
internal sealed class SavedLocationRepository(ApplicationDbContext context) :
    GenericRepository<SavedLocation>(context), ISavedLocationRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<SavedLocation?> GetByRiderAndLocationTypeAsync(long riderId, SavedLocationType locationType) =>
        await _context.Set<SavedLocation>()
        .FirstOrDefaultAsync(x => x.RiderId == riderId && x.LocationType == locationType);

    public async Task<ICollection<SavedLocationModel>> GetByRiderAsync(long riderId) =>
        await _context.Set<SavedLocation>()
        .Where(x => x.RiderId == riderId)
        .Select(x => new SavedLocationModel
        {
            Id = x.Id,
            Coordinates = x.GetCoordinates(),
            Address = x.Address
        })
        .ToListAsync();
}
