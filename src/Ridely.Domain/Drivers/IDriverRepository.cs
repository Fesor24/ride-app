using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Domain.Drivers;
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
