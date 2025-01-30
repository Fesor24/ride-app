using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ridely.Domain.Abstractions;
using Ridely.Api.Shared;

namespace Ridely.Api.Filter;

[AttributeUsage(AttributeTargets.All)]
public class ResourceAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        IConfiguration config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        string? headerApiKey = context.HttpContext.Request.Headers["x-api-key"];

        var failureResult = new UnauthorizedObjectResult(new ApiResponse(new Error(
            "unauthorized",
            "Invalid api key")));

        if (string.IsNullOrWhiteSpace(headerApiKey))
            context.Result = failureResult;

        string apiKey = config["Ridely:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentNullException(nameof(apiKey), "Api key not found");

        if (apiKey != headerApiKey)
            context.Result = failureResult;
    }
}
