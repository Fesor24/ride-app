using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Rides;

namespace Soloride.Application.Abstractions.Payment;
public interface IPaymentService
{
    Task<Result> ProcessCardTripPaymentAsync(Ride ride,
        CancellationToken cancellationToken = default);
    Task<(long RideFare, long AmountDueByRider)> ProcessPaymentAndDriverCommissionAsync(Driver driver, Ride ride,
        CancellationToken cancellationToken = default);
    Task RefundAsync(Ride ride, Ulid reference);
}
