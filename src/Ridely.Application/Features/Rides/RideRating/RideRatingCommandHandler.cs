using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.RideRating;
internal sealed class RideRatingCommandHandler:
    ICommandHandler<RideRatingCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IRatingsRepository _ratingsRepository;

    public RideRatingCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository,
        IRideRepository rideRepository, IRatingsRepository ratingsRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
        _rideRepository = rideRepository;
        _ratingsRepository = ratingsRepository;
    }

    public async Task<Result<bool>> Handle(RideRatingCommand request, CancellationToken cancellationToken)
    {
        if (request.Rating > 5) return Error.BadRequest("rating.invalid", "Rating can not be more than 5");

        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        var rideRatingExist = await _ratingsRepository
            .GetByRideAsync(ride.Id) != null;

        if (rideRatingExist) return Error.BadRequest("ride.rated", "This ride has been rated");

        List<RideStatus> rideStatuses = [
            RideStatus.Completed,
            RideStatus.Reassigned
            ];

        if (!rideStatuses.Any(status => status == ride.Status))
            return Error.BadRequest("ride.notcompleted", "Only completed or reassigned rides can be rated");

        Ratings rideRatings = new(request.Rating, ride.Id, request.Feedback);

        var driver = await _driverRepository
            .GetAsync(ride.DriverId!.Value);

        if (driver is not null)
        {
            driver.UpdateRatings(request.Rating);

            _driverRepository.Update(driver);
        }

        await _ratingsRepository
            .AddAsync(rideRatings);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
