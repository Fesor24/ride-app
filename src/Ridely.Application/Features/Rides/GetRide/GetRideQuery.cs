using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.GetRide;
public sealed record GetRideQuery(int RideId) : IQuery<GetRideResponse>;
