using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.InitiateEmailVerification;
public sealed record InitiateEmailVerificationCommand(long? DriverId, long? RiderId) : ICommand;
