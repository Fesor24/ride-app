using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.EndRide;
public sealed record EndRideCommand(long RideId) :
    ICommand<EndRideResponse>;
