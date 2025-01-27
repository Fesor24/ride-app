using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Admin.Rides.GetRideLogs;
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
