using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.StartRide;
public sealed record StartRideCommand(long RideId) :
    ICommand<StartRideResponse>;
