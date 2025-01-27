using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Extensions;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Admin.Rides.GetRideLogs;
internal sealed class GetRideLogsQueryHandler : IQueryHandler<GetRideLogsQuery, IReadOnlyList<GetRideLogsResponse>>
{
    private readonly IRideLogRepository _rideLogRepository;

    public GetRideLogsQueryHandler(IRideLogRepository rideLogRepository)
    {
        _rideLogRepository = rideLogRepository;
    }

    public async Task<Result<IReadOnlyList<GetRideLogsResponse>>> Handle(GetRideLogsQuery request, 
        CancellationToken cancellationToken)
    {
        var rideLogs = await _rideLogRepository.GetLogsByRide(request.RideId);

        return rideLogs.Select(log => new GetRideLogsResponse(
            log.Status.ToString(),
            log.CreatedAtUtc.ToCustomDateString()
            )).ToList();
    }
}
