using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.CancelRideRequest;
public sealed record CancelRideRequestCommand(long RideId, 
    string CancellationReason, bool? SystemInvoked = false, UserType CancelledBy = UserType.Rider) :
    ICommand;
