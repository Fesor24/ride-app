using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Users.UpdateDeviceToken;
public sealed  record UpdateDeviceTokenCommand(string DeviceTokenId, long? RiderId, long? DriverId)
    : ICommand;
