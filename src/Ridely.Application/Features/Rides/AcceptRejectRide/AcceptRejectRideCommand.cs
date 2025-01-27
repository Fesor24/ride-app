using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.AcceptRejectRide;
public sealed record AcceptRejectRideCommand(
    long RideId,
    long DriverId,
    bool AcceptRide) :
    ICommand<AcceptRejectResponse>;
