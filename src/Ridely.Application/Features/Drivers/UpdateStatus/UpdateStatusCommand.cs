using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.UpdateStatus;
public sealed record UpdateStatusCommand(UpdateDriverStatusEnum Status, long DriverId)
    : ICommand;




