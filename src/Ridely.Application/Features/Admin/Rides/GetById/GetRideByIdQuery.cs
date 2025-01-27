using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Rides.GetById;
public sealed record GetRideByIdQuery(long RideId) : IQuery<GetRideResponse>;