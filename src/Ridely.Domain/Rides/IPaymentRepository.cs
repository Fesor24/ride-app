using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Rides;
public interface IPaymentRepository : IGenericRepository<Payment>
{
    Task<Payment?> GetByReferenceAsync(Ulid reference);
}
