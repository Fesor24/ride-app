using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Riders;

namespace Soloride.Domain.Riders;
public interface IRiderRepository : IGenericRepository<Rider>
{
    Task<Rider?> GetByPhoneNoAsync(string phoneNo);
    Task<RiderModel?> GetDetailsAsync(long riderId);
    Task<PaginatedList<RiderModel>> Search(RiderSearchParams searchParams);
    Task<string?> GetPhoneNoAsync(long riderId);
    Task<Rider?> GetByEmailAsync(string email);
    Task<Rider?> GetByReferralCodeAsync(string referralCode);
    Task<int> GetTotalCountAsync();
}
