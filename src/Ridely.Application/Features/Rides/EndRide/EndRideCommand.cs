using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.EndRide;
public sealed record EndRideCommand(long RideId) :
    ICommand<EndRideResponse>;
