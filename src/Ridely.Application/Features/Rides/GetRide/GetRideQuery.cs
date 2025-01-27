using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Rides.GetRide;
public sealed record GetRideQuery(int RideId) : IQuery<GetRideResponse>;
