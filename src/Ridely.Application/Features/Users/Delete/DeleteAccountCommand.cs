using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Users.Delete;
public sealed record DeleteAccountCommand(long? RiderId, long? DriverId) :
    ICommand;
