using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;

namespace Ridely.Domain.Transactions;
public interface IPaymentCardRepository : IGenericRepository<PaymentCard>
{
    Task<IReadOnlyList<PaymentCard>> GetAllByRiderAsync(long riderId);
}
