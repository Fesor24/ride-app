using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Users;

namespace Soloride.Application.Features.Admin.Users.Query.GetDashboard;
internal sealed class GetDashboardQueryHandler : 
    IQueryHandler<GetDashboardQuery, GetDashboardResponse>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideRepository _rideRepository;

    public GetDashboardQueryHandler(IRoleRepository roleRepository, IRiderRepository riderRepository,
        IDriverRepository driverRepository, IRideRepository rideRepository)
    {
        _roleRepository = roleRepository;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideRepository = rideRepository;
    }

    public async Task<Result<GetDashboardResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository
            .GetAsync(request.RoleId);

        if (role is null) return Error.NotFound("ROLE_NOTFOUND", "Role not found");

        if (role.Code != nameof(RoleCategory.Admin)) return Error.BadRequest("unauthorized", "Access only to admin");

        return new GetDashboardResponse
        {
            User = new DashboardUserResponse
            {
                Riders = await _riderRepository.GetTotalCountAsync(),
                Drivers = await _driverRepository.GetTotalCountAsync()
            },
            Ride = new DashboardRideResponse
            {
                CompletedRides = await _rideRepository.GetTotalCountAsync(RideStatus.Completed),
                ReroutedRides = await _rideRepository.GetTotalCountAsync(RideStatus.Rerouted),
                RidesRequested = await _rideRepository.GetTotalCountAsync(RideStatus.Requested),
                ReassignedRides = await _rideRepository.GetTotalCountAsync(RideStatus.Reassigned),
                RidesInTransit = await _rideRepository.GetTotalCountAsync(RideStatus.InTransit),
                CancelledRides = await _rideRepository.GetTotalCountAsync(RideStatus.Cancelled)
            }
        };
    }
}
