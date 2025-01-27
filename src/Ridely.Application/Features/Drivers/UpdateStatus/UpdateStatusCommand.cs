using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.UpdateStatus;
public sealed record UpdateStatusCommand(UpdateDriverStatusEnum Status, long DriverId)
    : ICommand;




