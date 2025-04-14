using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public interface IPaymentDetailRepository : IGenericRepository<PaymentDetail>
{
    Task<PaymentDetail?> GetAsync(PaymentFor paymentFor, long paymentId);
    Task<PaymentDetail?> GetByReferenceAsync(Ulid reference);
    Task<List<PaymentDetail>> GetAllByPaymentId(long paymentId);
    Task<IEnumerable<PaymentDetail>> GetAllByReference(List<Ulid> paymentReferences);
}
