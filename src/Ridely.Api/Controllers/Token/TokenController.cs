using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Application.Features.Accounts.Token;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Dto.Account;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Token;

[ResourceAuthorizationFilter]
public class TokenController : BaseController<TokenController>
{
    [HttpPost("api/token")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetToken(TokenDto tokenDto)
    {
        var response = await Sender.Send(new GetAccessTokenCommand(tokenDto.AccessToken, tokenDto.RefreshToken));

        return response.Match(value => Ok(value), this.HandleErrorResult);
    }
}
