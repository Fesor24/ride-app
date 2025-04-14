using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.VerifyCardPayment;
public sealed record VerifyCardPaymentCommand(long RideId) : ICommand;
