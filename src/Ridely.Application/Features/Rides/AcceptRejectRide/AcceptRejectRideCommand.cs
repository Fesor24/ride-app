using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.AcceptRejectRide;
public sealed record AcceptRejectRideCommand(
    long RideId,
    long DriverId,
    bool AcceptRide) :
    ICommand<AcceptRejectResponse>;
