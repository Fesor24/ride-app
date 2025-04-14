using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers;
public interface IDriverDiscountRepository : IGenericRepository<DriverDiscount>
{
    Task<DriverDiscount?> GetDiscountByDriverAndTypeAsync(long driverId, DriverDiscountType type);
}
