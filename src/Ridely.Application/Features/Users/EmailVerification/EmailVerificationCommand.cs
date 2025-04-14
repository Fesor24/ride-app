using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.EmailVerification;
public sealed record EmailVerificationCommand(long? DriverId, long? RiderId) : ICommand;
