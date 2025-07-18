﻿using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Common.Banks.Query.GetAll;
using Ridely.Application.Features.Common.GetContacts;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Common;

[ResourceAuthorizationFilter]
public class CommonController : BaseController<CommonController>
{
    [HttpGet("api/banks")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<List<GetBankResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetBanks()
    {
        var res = await Sender.Send(new GetBanksQuery());

        return res.Match(value => Ok(new ApiResponse<List<GetBankResponse>>(value)),
            this.HandleErrorResult);
    }

    [HttpGet("api/contacts")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetContactsResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> ContactSupport()
    {
        var res = await Sender.Send(new GetContactSupportQuery());

        return res.Match(value => Ok(new ApiResponse<GetContactsResponse>(value)), this.HandleErrorResult);
    }
}
