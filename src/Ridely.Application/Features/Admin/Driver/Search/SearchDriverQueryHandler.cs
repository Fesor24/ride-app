using AutoMapper;
using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Application.Features.Admin.Driver.Search;
internal class SearchDriverQueryHandler(IDriverRepository driverRepository, IMapper mapper) :
    IRequestHandler<SearchDriverQuery, Result<PaginatedList<SearchDriverResponse>>>
{
    public async Task<Result<PaginatedList<SearchDriverResponse>>> Handle(SearchDriverQuery request,
        CancellationToken cancellationToken)
    {
        var searchParams = new DriverSearchParams
        {
            CabType = request.CabType,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            Email = request.Email,
            PhoneNo = request.PhoneNo
        };

        var response = await driverRepository.Search(searchParams);

        return new PaginatedList<SearchDriverResponse>
        {
            PageNumber = response.PageNumber,
            PageSize = response.PageSize,
            TotalItems = response.TotalItems,
            Items = mapper.Map<List<SearchDriverResponse>>(response.Items)
        };
    }
}
