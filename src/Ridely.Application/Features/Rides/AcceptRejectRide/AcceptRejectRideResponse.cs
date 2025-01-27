using Ridely.Application.Models.Shared;

namespace Ridely.Application.Features.Rides.AcceptRejectRide;
public sealed record AcceptRejectResponse(LocationResponse RiderLocation,
    bool Accepted, bool RideRequestCancelled);
