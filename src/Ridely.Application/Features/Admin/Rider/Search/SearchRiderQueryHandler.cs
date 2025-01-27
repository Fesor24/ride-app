using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Extensions;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Riders;
using Soloride.Domain.Riders;

namespace Soloride.Application.Features.Admin.Rider.Query.Search;
internal sealed class SearchRiderQueryHandler(IRiderRepository riderRepository) : 
    IQueryHandler<SearchRiderQuery, PaginatedList<SearchRiderResponse>>
{
    public async Task<Result<PaginatedList<SearchRiderResponse>>> Handle(SearchRiderQuery request, 
        CancellationToken cancellationToken)
    {
        var searchParams = new RiderSearchParams
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            From = request.From,
            To = request.To,
            PhoneNo = request.PhoneNo,
            Email = request.Email,
        };

        var result = await riderRepository
            .Search(searchParams);

        return new PaginatedList<SearchRiderResponse>
        {
            Items = result.Items.ConvertAll(rider => new SearchRiderResponse
            {
                FirstName = rider.FirstName,
                LastName = rider.LastName,
                CreatedAt = rider.CreatedAt.ToCustomDateString(),
                Email = rider.Email,
                PhoneNo = rider.PhoneNo
            }),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
