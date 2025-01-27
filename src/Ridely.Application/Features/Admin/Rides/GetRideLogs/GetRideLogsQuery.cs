using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Rides.GetRideLogs;
public sealed record GetRideLogsQuery(long RideId): IQuery<IReadOnlyList<GetRideLogsResponse>>;
