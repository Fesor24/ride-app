using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Services;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Common.Calls.Command;

internal sealed class RouteCallCommandHandler : 
    ICommandHandler<RouteCallCommand, string?>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideRepository _rideRepository;
    private readonly ICacheService _cacheService;

    public RouteCallCommandHandler(IDriverRepository driverRepository, IRiderRepository riderRepository,
        IRideRepository rideRepository, ICacheService cacheService)
    {
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _rideRepository = rideRepository;
        _cacheService = cacheService;
    }

    public async Task<Result<string?>> Handle(RouteCallCommand request, CancellationToken cancellationToken)
    {
        string callKey = Cache.Calls.Key(request.PhoneNo);

        var value = await _cacheService.GetAsync(callKey);

        if (string.IsNullOrWhiteSpace(value))
            return Error.NotFound("record.notfound", "Record not found");

        string user = value.Split("-")[0];
        
        string rideId = value.Split("-")[1];

        var ride = await _rideRepository
            .GetAsync(int.Parse(rideId));
        
        if(ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (user == "rider")
        {
            if (!ride.DriverId.HasValue)
                return Error.NotFound("driver.notfound", "Driver not found");
            
            return await _driverRepository
                .GetPhoneNoAsync(ride.DriverId.Value);
        }
        else if (user == "driver")
        {
            return await _riderRepository
                .GetPhoneNoAsync(ride.RiderId);
        }

        return string.Empty;
    }
}