using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.GetRide;
internal sealed class GetRideQueryHandler(IRideRepository rideRepository, IRatingsRepository ratingsRepository) :
    IQueryHandler<GetRideQuery, GetRideResponse>
{
    public async Task<Result<GetRideResponse>> Handle(GetRideQuery request, CancellationToken cancellationToken)
    {
        var ride = await rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        return new GetRideResponse
        {
            Driver = new RideResponseDriverObj
            {
                Name = ride.Driver?.FirstName ?? " " + " " + ride.Driver?.LastName ?? "",
                ProfileImageUrl = ride.Driver?.ProfileImageUrl ?? "",
                Cab = new RideResponseVehicleObj
                {
                    Manufacturer = ride.Driver?.Cab?.Manufacturer ?? "",
                    Model = ride.Driver?.Cab?.Model ?? "",
                    Color = ride.Driver?.Cab?.Color ?? ""
                }
            },
            Rider = new RideResponseRiderObj
            {
                Name = ride.Rider.FirstName + " " + ride.Rider.LastName
            },
            Ride = new RideResponseRideObj
            {
                Amount = ride.Payment.Amount,
                PaymentMethod = ride.Payment.Method.ToString(),
                DistanceInMeters = ride.DistanceInMeters,
                Rating = (await ratingsRepository.GetByRideAsync(ride.Id))?.Rating ?? 0,
                CreatedAt = ride.CreatedAtUtc.ToCustomDateString(),
                Destination = ride.DestinationAddress,
                Source = ride.SourceAddress,
                EstimatedFare = ride.EstimatedFare
            }
        };

    }
}
