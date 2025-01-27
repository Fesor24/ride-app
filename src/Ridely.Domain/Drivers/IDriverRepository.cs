using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Drivers;

namespace Soloride.Domain.Drivers;
public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<Driver?> GetDetailsAsync(long driverId);
    Task<Driver?> GetByPhoneNoAsync(string phoneNo);
    Task<Driver?> GetByReferralCodeAsync(string referralCode);
    Task<PaginatedList<DriverModel>> Search(DriverSearchParams searchParams);
    Task<string?> GetPhoneNoAsync(long driverId);
    Task<Driver?> GetByEmailAsync(string email);
    Task<int> GetTotalCountAsync();
}
