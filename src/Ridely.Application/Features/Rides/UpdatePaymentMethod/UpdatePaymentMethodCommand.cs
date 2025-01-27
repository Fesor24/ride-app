using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Rides.UpdatePaymentMethod;
public sealed record UpdatePaymentMethodCommand(
    long RideId,
    PaymentMethod PaymentMethod,
    long? PaymentCardId) :
    ICommand;
