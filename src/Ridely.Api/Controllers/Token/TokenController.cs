using Microsoft.AspNetCore.Mvc;
using Soloride.Application.Features.Accounts.Login;
using Soloride.Application.Features.Accounts.Token;
using SolorideAPI.Controllers.Base;
using SolorideAPI.Dto.Account;
using SolorideAPI.Extensions;
using SolorideAPI.Filter;
using SolorideAPI.Shared;

namespace SolorideAPI.Controllers.Token;

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
