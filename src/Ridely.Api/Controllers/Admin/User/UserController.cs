using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Admin.Users.Query.GetDashboard;
using Ridely.Application.Features.Admin.Users.Query.GetProfile;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Admin.User;

[Route("api/user")]
public class UserController : AdminBaseController<UserController>
{
    [HttpGet($"profile")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetProfileResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetUserProfile()
    {
        var response = await Sender.Send(new GetProfileQuery(UserId));

        return response.Match(value => Ok(new ApiResponse<GetProfileResponse>(value)), this.HandleErrorResult);
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetDashboardResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetDashboard()
    {
        var response = await Sender.Send(new GetDashboardQuery(RoleId));

        return response.Match(value => Ok(new ApiResponse<GetDashboardResponse>(value)), this.HandleErrorResult);
    }
}
