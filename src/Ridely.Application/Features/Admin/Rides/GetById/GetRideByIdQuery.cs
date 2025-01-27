using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Rides.GetById;
public sealed record GetRideByIdQuery(long RideId) : IQuery<GetRideResponse>;