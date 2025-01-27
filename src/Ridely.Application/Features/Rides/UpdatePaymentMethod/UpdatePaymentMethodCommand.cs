using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.UpdatePaymentMethod;
public sealed record UpdatePaymentMethodCommand(
    long RideId,
    PaymentMethod PaymentMethod,
    long? PaymentCardId) :
    ICommand;
