using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Admin.Driver.GetByPhoneNo;
using Ridely.Application.Features.Admin.Driver.Search;
using Ridely.Application.Features.Admin.Driver.SearchTransactions;
using Ridely.Application.Features.Admin.Driver.VerifyDriver;
using Ridely.Domain.Models;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Admin.Driver;

[Route("api/drivers")]
public class DriverController : AdminBaseController<DriverController>
{
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchDriverResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchDriver([FromQuery] SearchDriverRequest searchParams)
    {
        var response = await Sender.Send(new SearchDriverQuery
        {
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            Email = searchParams.Email,
            PhoneNo = searchParams.PhoneNo
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchDriverResponse>>(value)),
            this.HandleErrorResult);
    }

    [HttpGet("{phoneNo}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetDriverByPhoneNoResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetDriver(string phoneNo)
    {
        var response = await Sender.Send(new GetDriverByPhoneNoQuery(phoneNo));

        return response.Match(value => Ok(new ApiResponse<GetDriverByPhoneNoResponse>(value)),
           this.HandleErrorResult);
    }

    [HttpPut("verify/{phoneNo}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> VerifyDriver(string phoneNo)
    {
        var response = await Sender.Send(new VerifyDriverCommand(phoneNo));

        return response.Match(value => Ok(new ApiResponse<bool>(value)),
           this.HandleErrorResult);
    }

    [HttpPost("transactions")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchDriverTransactionResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchRiderTransactions(SearchDriverTransactionRequest request)
    {
        var response = await Sender.Send(new SearchDriverTransactionQuery
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

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchDriverTransactionResponse>>(value)),
            this.HandleErrorResult);
    }
}
