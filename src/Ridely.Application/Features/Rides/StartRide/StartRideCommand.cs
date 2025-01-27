using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.StartRide;
public sealed record StartRideCommand(long RideId) :
    ICommand<StartRideResponse>;
