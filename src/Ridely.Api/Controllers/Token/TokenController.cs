using Microsoft.AspNetCore.Mvc;
using Ridely.Api.Controllers.Base;
using Ridely.Api.Dto.Account;
using Ridely.Api.Extensions;
using Ridely.Api.Filter;
using Ridely.Api.Shared;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Application.Features.Accounts.Token;

namespace Ridely.Api.Controllers.Token;

[ResourceAuthorizationFilter]
public class TokenController : BaseController<TokenController>
{
    //private readonly WebPubSubServiceClient<MainApplicationHub> _webPubSubServiceClient;

    [HttpPost("api/token")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<LoginResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetToken(TokenDto tokenDto)
    {
        var response = await Sender.Send(new GetAccessTokenCommand(tokenDto.AccessToken, tokenDto.RefreshToken));

        return response.Match(value => Ok(value), this.HandleErrorResult);
    }
    
    //[HttpGet("api/negotiate")]
    //[Authorize]
    //public async Task<IActionResult> GetWebSocketToken()
    //{
    //    var driverId = GetDriverId();
    //    var riderId = GetRiderId();

    //    string userId = "";

    //    if(driverId.HasValue)
    //        userId = WebSocketKeys.Driver.Key(driverId.Value.ToString());

    //    else if(riderId.HasValue)
    //        userId = WebSocketKeys.Rider.Key(riderId.Value.ToString());

    //    var accessUri = _webPubSubServiceClient.GetClientAccessUri(userId: userId);

    //    return Ok(new
    //    {
    //        Uri = await Task.FromResult(accessUri),
    //    });
    //}
}
