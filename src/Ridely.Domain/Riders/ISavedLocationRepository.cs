using Soloride.Domain.Abstractions;
using Soloride.Domain.Models.Common;

namespace Soloride.Domain.Riders;
public interface ISavedLocationRepository : IGenericRepository<SavedLocation>
{
    Task<ICollection<SavedLocationModel>> GetByRiderAsync(long riderId);
    Task<SavedLocation?> GetByRiderAndLocationTypeAsync(long riderId, SavedLocationType locationType);
}
