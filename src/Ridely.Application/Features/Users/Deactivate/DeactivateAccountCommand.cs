using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.Deactivate;
public sealed record DeactivateAccountCommand(long? DriverId = null, long? RiderId = null) :
    ICommand;
