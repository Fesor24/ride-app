using AutoMapper;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.Search;
internal sealed class SearchRideQueryHandler(IRideRepository rideRepository) :
    IQueryHandler<SearchRideQuery, PaginatedList<SearchRideResponse>>
{
    public async Task<Result<PaginatedList<SearchRideResponse>>> Handle(SearchRideQuery request,
        CancellationToken cancellationToken)
    {
        var searchParams = new RideSearchParams
        {
            DriverId = request.DriverId,
            RiderId = request.RiderId,
            From = request.From,
            To = request.To,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            RideStatus = request.RideStatus
        };

        var res = await rideRepository.Search(searchParams);

        return new PaginatedList<SearchRideResponse>
        {
            Items = res.Items.ConvertAll(ride => new SearchRideResponse
            {
                Amount = ride.Amount,
                PaymentMethod = ride.PaymentMethod.ToString(),
                CreatedAt = ride.CreatedAt.ToCustomDateString(),
                Destination = ride.Destination,
                Id = ride.Id,
                Source = ride.Source,
            }),
            PageNumber = res.PageNumber,
            PageSize = res.PageSize,
            TotalItems = res.TotalItems,
        };
    }
}
