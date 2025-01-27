using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Rides;

namespace Ridely.Application.Abstractions.Payment;
public interface IPaymentService
{
    Task<Result> ProcessCardTripPaymentAsync(Ride ride,
        CancellationToken cancellationToken = default);
    Task<(long RideFare, long AmountDueByRider)> ProcessPaymentAndDriverCommissionAsync(Driver driver, Ride ride,
        CancellationToken cancellationToken = default);
    Task RefundAsync(Ride ride, Ulid reference);
}
