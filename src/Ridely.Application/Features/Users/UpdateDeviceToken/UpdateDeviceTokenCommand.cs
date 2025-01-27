using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.UpdateDeviceToken;
public sealed  record UpdateDeviceTokenCommand(string DeviceTokenId, long? RiderId, long? DriverId)
    : ICommand;
