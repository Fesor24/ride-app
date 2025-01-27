using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Admin.Rides.GetById;
internal sealed class GetRideByIdQueryHandler : IQueryHandler<GetRideByIdQuery, GetRideResponse>
{
    private readonly IRideRepository _rideRepository;

    public GetRideByIdQueryHandler(IRideRepository rideRepository)
    {
        _rideRepository = rideRepository;
    }
    public async Task<Result<GetRideResponse>> Handle(GetRideByIdQuery request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetRideDetails(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        return new GetRideResponse
        {
            Driver = ride.Driver is not null ? ride.Driver.FirstName + " " + ride.Driver.LastName : "",
            Rider = ride.Rider.FirstName + " " + ride.Rider.LastName,
            Amount = ride.EstimatedFare,
            Source = ride.SourceAddress,
            Destination = ride.DestinationAddress,
            PaymentMethod = ride.Payment.Method.ToString(),
            Status = ride.Status.ToString(),
            RideId = ride.Id,
            CreatedAt = ride.CreatedAtUtc.ToCustomDateString(),
            PaymentStatus = ride.Payment.Status.ToString()
        };
    }
}
