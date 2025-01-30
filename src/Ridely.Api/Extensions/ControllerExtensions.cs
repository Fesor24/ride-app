using Microsoft.AspNetCore.Mvc;
using Ridely.Domain.Abstractions;
using Ridely.Api.Shared;

namespace Ridely.Api.Extensions;

internal static class ControllerExtensions
{
    internal static IActionResult HandleErrorResult(this ControllerBase controller, Error error)
    {
        var errorType = error.GetType();

        if(errorType == typeof(NotFound))
            return controller.NotFound(new ApiResponse(error));

        else if(errorType == typeof(Unauthorized))
            return controller.Unauthorized(new ApiResponse(error));

        else 
            return controller.BadRequest(new ApiResponse(error));
    }
}
