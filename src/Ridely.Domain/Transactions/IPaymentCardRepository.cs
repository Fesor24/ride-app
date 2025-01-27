using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;

namespace Soloride.Domain.Transactions;
public interface IPaymentCardRepository : IGenericRepository<PaymentCard>
{
    Task<IReadOnlyList<PaymentCard>> GetAllByRiderAsync(long riderId);
}
