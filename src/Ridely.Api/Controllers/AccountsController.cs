using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Api.Controllers.Base;
using Ridely.Api.Dto.Account;
using Ridely.Api.Extensions;
using Ridely.Api.Filter;
using Ridely.Api.Shared;

namespace Ridely.Api.Controllers;

[ResourceAuthorizationFilter]
public class AccountsController : BaseController<AccountsController>
{
    [HttpPost("api/login")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var response = await Sender.Send(new LoginCommand(
            loginDto.Email, loginDto.Password));

        return response.Match(value => Ok(new ApiResponse<LoginResponse>(value)), this.HandleErrorResult);
    }
}
