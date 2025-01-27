using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Users.Deactivate;
public sealed record DeactivateAccountCommand(long? DriverId = null, long? RiderId = null) :
    ICommand;
