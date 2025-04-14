using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public interface IPaymentRepository : IGenericRepository<Payment>
{
    //Task<Payment?> GetByReferenceAsync(Ulid reference);
}
