using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.CancelRideRequest;
public sealed record CancelRideRequestCommand(long RideId, 
    string CancellationReason, bool? SystemInvoked = false) :
    ICommand;
