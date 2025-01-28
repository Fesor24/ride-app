using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebPubSub.AspNetCore;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Application.Features.Accounts.Token;
using Ridely.Infrastructure.WebSockets.Hub;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Dto.Account;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Token;

[ResourceAuthorizationFilter]
public class TokenController : BaseController<TokenController>
{
    private readonly WebPubSubServiceClient<MainApplicationHub> _webPubSubServiceClient;

    public TokenController(WebPubSubServiceClient<MainApplicationHub> webPubSubServiceClient)
    {
        _webPubSubServiceClient = webPubSubServiceClient;
    }

    [HttpPost("api/token")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetToken(TokenDto tokenDto)
    {
        var response = await Sender.Send(new GetAccessTokenCommand(tokenDto.AccessToken, tokenDto.RefreshToken));

        return response.Match(value => Ok(value), this.HandleErrorResult);
    }
    // rename to negotiate
    [HttpGet("api/ws-token")]
    public async Task<IActionResult> GetWebSocketToken()
    {
        var userId = "DR-5";

        var accessUri = _webPubSubServiceClient.GetClientAccessUri(userId: userId);

        return Ok(new
        {
            Uri = await Task.FromResult(accessUri),
        });
    }
}
