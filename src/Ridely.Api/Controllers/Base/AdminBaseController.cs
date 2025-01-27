using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SolorideAPI.Filter;

namespace SolorideAPI.Controllers.Base
{
    [ApiExplorerSettings(GroupName = SwaggerGroupNames.Admin)]
    [ApiController]
    [Authorize]
    [ResourceAuthorizationFilter]
    public class AdminBaseController<TController>: ControllerBase
    {
        protected ISender Sender => HttpContext.RequestServices.GetRequiredService<ISender>();

        protected long UserId
        {
            get
            {
                var identifier = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (identifier is null)
                    throw new Exception("User is unauthorized");

                return long.Parse(identifier);
            }
        }

        protected long RoleId
        {
            get
            {
                var roleIdentifier = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;

                if (roleIdentifier is null)
                    throw new Exception("No role found");

                return long.Parse(roleIdentifier);
            }
        }
            
    }
}
