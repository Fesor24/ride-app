using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ridely.Shared.Constants;

namespace RidelyAPI.Controllers.Base;

[ApiController]
public class BaseController<TController> : ControllerBase
{
    private ISender _sender;

    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected long DriverId
    {
        get
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstant.Driver) 
                ?? throw new ApiUnauthorizedException("User not authorized");

            return long.Parse(claim.Value);
        }
    }

    protected long RiderId
    {
        get
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstant.Rider) ??
                throw new ApiUnauthorizedException("User not authorized");

            return long.Parse(claim.Value);
        }
    }

    protected long UserId
    {
        get
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier) ??
                throw new ApiUnauthorizedException("User not authorized");

            return long.Parse(claim.Value);
        }
    }

    protected long RoleId
    {
        get
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role) 
                ?? throw new ApiUnauthorizedException("User not authorized");

            return long.Parse(claim.Value);
        }
    }

    protected long? GetDriverId()
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstant.Driver);

        return claim is null ? null : long.Parse(claim.Value);
    }

    protected long? GetRiderId()
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstant.Rider);

        return claim is null ? null : long.Parse(claim.Value);
    }
}
