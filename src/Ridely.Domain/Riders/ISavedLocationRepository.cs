using Ridely.Domain.Abstractions;
using Ridely.Domain.Models.Common;

namespace Ridely.Domain.Riders;
public interface ISavedLocationRepository : IGenericRepository<SavedLocation>
{
    Task<ICollection<SavedLocationModel>> GetByRiderAsync(long riderId);
    Task<SavedLocation?> GetByRiderAndLocationTypeAsync(long riderId, SavedLocationType locationType);
}
