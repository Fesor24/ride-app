using Ridely.Application.Models.Shared;

namespace Ridely.Application.Features.Rides.RideRequest;
public sealed record RideRequestResponse(
    bool DriversAvailable
    );
