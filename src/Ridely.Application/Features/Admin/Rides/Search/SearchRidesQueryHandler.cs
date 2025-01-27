using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Extensions;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Rides;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Admin.Rides.Search;
internal sealed class SearchRidesQueryHandler : 
    IQueryHandler<SearchRidesQuery, PaginatedList<SearchRidesResponse>>
{
    private readonly IRideRepository _rideRepository;

    public SearchRidesQueryHandler(IRideRepository rideRepository)
    {
        _rideRepository = rideRepository;
    }

    public async Task<Result<PaginatedList<SearchRidesResponse>>> Handle(SearchRidesQuery request, 
        CancellationToken cancellationToken)
    {
        var searchParams = new RideSearchParams
        {
            DriverPhoneNo = request.DriverPhoneNo,
            RiderPhoneNo = request.RiderPhoneNo,
            RideStatus = request.Status,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            From = request.From,
            To = request.To
        };

        var result = await _rideRepository.Search(searchParams);

        return new PaginatedList<SearchRidesResponse>
        {
            Items = result.Items.ConvertAll(ride => new SearchRidesResponse
            {
                Driver = ride.Driver,
                Rider = ride.Rider,
                PaymentStatus = ride.PaymentStatus.ToString(),
                Amount = ride.Amount,
                Status = ride.RideStatus.ToString(),
                CreatedAt = ride.CreatedAt.ToCustomDateString(),
                RideId = ride.Id,
            }),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
