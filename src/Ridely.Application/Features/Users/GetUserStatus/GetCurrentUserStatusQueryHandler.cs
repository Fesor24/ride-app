using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Users.GetUserStatus;
internal sealed class GetCurrentUserStatusQueryHandler:
    IQueryHandler<GetCurrentUserStatusQuery, GetCurrentUserStatusResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideRepository _rideRepository;

    public GetCurrentUserStatusQueryHandler(IUnitOfWork unitOfWork, IRiderRepository riderRepository,
        IDriverRepository driverRepository, IRideRepository rideRepository)
    {
        _unitOfWork = unitOfWork;
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideRepository = rideRepository;
    }

    public async Task<Result<GetCurrentUserStatusResponse>> Handle(GetCurrentUserStatusQuery request,
        CancellationToken cancellationToken)
    {
        if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            if(!rider.CurrentRideId.HasValue) return new GetCurrentUserStatusResponse
            {
                Status = CurrentUserStatusEnum.Default
            };

            var ride = await _rideRepository
                .GetRideDetails(rider.CurrentRideId.Value);

            if (ride is null)
                return Error.NotFound("ride.notfound", "Latest ride not found");

            return new GetCurrentUserStatusResponse
            {
                Status = CurrentUserStatusEnum.Ride,
                Ride = new GetCurrentUserStatusResponse.CurrentUserRide
                {
                    RideStatus = ride.Status,
                    RideId = ride.Id,
                    Destination = ride.DestinationAddress,
                    Source = ride.SourceAddress,
                    SourceCoordinates = ride.GetCoordinates(ride.SourceCordinates),
                    DestinationCoordinates = ride.GetCoordinates(ride.DestinationCordinates),
                    PaymentMethod = ride.Payment.Method,
                    FareEstimate = ride.EstimatedFare,
                    MusicGenre = ride.MusicGenre.ToString(),
                    RideConversation = ride.HaveConversation,
                    Driver = ride.DriverId.HasValue ? new()
                    {
                        FirstName = ride.Driver.FirstName,
                        LastName = ride.Driver.LastName,
                        ProfileImageUrl = ride.Driver.ProfileImageUrl,
                        Cab = new()
                        {
                            LicensePlateNo = ride.Driver.Cab.LicensePlateNo,
                            Color = ride.Driver.Cab.Color,
                            Manufacturer = ride.Driver.Cab.Manufacturer,
                            Model = ride.Driver.Cab.Model
                        }
                    } : new()
                }
            };

        }
        else if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository
                .GetAsync(request.DriverId.Value);

            if (driver is null)
                return Error.NotFound("driver.notfound", "Driver not found");

            if (!driver.CurrentRideId.HasValue) return new GetCurrentUserStatusResponse
            {
                Status = CurrentUserStatusEnum.Default
            };

            var ride = await _rideRepository
               .GetRideDetails(driver.CurrentRideId.Value);

            if (ride is null)
                return Error.NotFound("ride.notfound", "Latest ride not found");

            return new GetCurrentUserStatusResponse
            {
                Status = CurrentUserStatusEnum.Ride,
                Ride = new()
                {
                    Destination = ride.DestinationAddress,
                    Source = ride.SourceAddress,
                    DestinationCoordinates = ride.GetCoordinates(ride.DestinationCordinates),
                    SourceCoordinates = ride.GetCoordinates(ride.SourceCordinates),
                    PaymentMethod = ride.Payment.Method,
                    Driver = new()
                    {
                        FirstName = ride.Driver.FirstName,
                        LastName = ride.Driver.LastName,
                        ProfileImageUrl = ride.Driver.ProfileImageUrl,
                        Cab = new()
                        {
                            LicensePlateNo = ride.Driver.Cab.LicensePlateNo,
                            Color = ride.Driver.Cab.Color,
                            Manufacturer = ride.Driver.Cab.Manufacturer,
                            Model = ride.Driver.Cab.Model
                        }
                    },
                    FareEstimate = ride.EstimatedFare,
                    RideId = ride.Id,
                    Rider = ride.Rider.FirstName + " " + ride.Rider.LastName,
                    RideStatus = ride.Status,
                    MusicGenre = ride.MusicGenre.ToString(),
                    RideConversation = ride.HaveConversation
                }
            };
        }

        else
            return Error.BadRequest("invalid.user", "Specify driver or rider");
    }
}
