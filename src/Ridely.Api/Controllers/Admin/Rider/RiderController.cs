using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Admin.Rider.Query.Search;
using Ridely.Application.Features.Admin.Rider.SearchTransactions;
using Ridely.Domain.Models;
using Ridely.Api.Controllers.Base;
using Ridely.Api.Extensions;
using Ridely.Api.Shared;

namespace Ridely.Api.Controllers.Admin.Rider;

[Route("api/riders")]
public class RiderController : AdminBaseController<RiderController>
{
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchRiderResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchRider([FromQuery] SearchRiderRequest request)
    {
        var response = await Sender.Send(new SearchRiderQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            From = request.From,
            To = request.To,
            PhoneNo = request.PhoneNo,
            Email = request.Email,
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchRiderResponse>>(value)),
            this.HandleErrorResult);
    }

    [HttpPost("transactions")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchRiderTransactionResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchRiderTransactions(SearchRiderTransaction request)
    {
        var response = await Sender.Send(new SearchRiderTransactionQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            From = request.From,
            To = request.To,
            PhoneNo = request.PhoneNo,
            Status = request.Status,
            Types = request.Types,
            Reference = request.Reference,
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchRiderTransactionResponse>>(value)),
            this.HandleErrorResult);
    }
}
