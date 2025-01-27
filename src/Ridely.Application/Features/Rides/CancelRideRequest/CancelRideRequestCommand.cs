using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.CancelRideRequest;
public sealed record CancelRideRequestCommand(long RideId, 
    string CancellationReason, bool? SystemInvoked = false) :
    ICommand;
