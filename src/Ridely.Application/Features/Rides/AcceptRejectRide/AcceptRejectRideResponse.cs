using Soloride.Application.Models.Shared;

namespace Soloride.Application.Features.Rides.AcceptRejectRide;
public sealed record AcceptRejectResponse(LocationResponse RiderLocation,
    bool Accepted, bool RideRequestCancelled);
