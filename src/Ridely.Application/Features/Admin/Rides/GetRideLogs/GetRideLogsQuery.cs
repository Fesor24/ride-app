using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Rides.GetRideLogs;
public sealed record GetRideLogsQuery(long RideId): IQuery<IReadOnlyList<GetRideLogsResponse>>;
