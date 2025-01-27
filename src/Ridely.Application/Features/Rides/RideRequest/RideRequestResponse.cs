using Soloride.Application.Models.Shared;

namespace Soloride.Application.Features.Rides.RideRequest;
public sealed record RideRequestResponse(
    bool DriversAvailable
    );
