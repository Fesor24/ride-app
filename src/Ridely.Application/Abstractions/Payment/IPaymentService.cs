using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Rides;

namespace Ridely.Application.Abstractions.Payment;
public interface IPaymentService
{
    Task<Result> ProcessCardTripPaymentAsync(Ride ride, long amount,
        CancellationToken cancellationToken = default);
    Task<PaymentResponse> ProcessPaymentAndDriverCommissionAsync(Driver driver, Ride ride,
        CancellationToken cancellationToken = default);
    Task ProcessReroutedTripPaymentAsync(Ride ride, long completedDistanceFare, long remainderDistanceFare);
    Task RefundAsync(Ride ride, Ulid reference);
    Task VerifyCardPaymentAndUpdatePaymentMethodToCashIfFailAsync(Ride ride);
    Task UpdatePaymentMethodToCashAndNotifyUsersAsync(Ride rideDetails);
}
