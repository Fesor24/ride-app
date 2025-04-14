using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Common.UploadImage;
using Ridely.Domain.Abstractions;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Upload;

[Authorize]
[ResourceAuthorizationFilter]
public class UploadsController : BaseController<UploadsController>
{
    [HttpPost("api/upload/image")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> Upload(IFormFile image,
        [FromForm(Name = "fileType")] FileType fileType)
    {
        long identifier;

        if (fileType == FileType.RidersProfileImage)
        {
            identifier = RiderId;
        }
        else
        {
            identifier = DriverId;
        }

        var allowedExtensionsPattern = @"^(.*\.)(png|jpg|jpeg)$";

        if (!Regex.IsMatch(image.FileName, allowedExtensionsPattern, RegexOptions.IgnoreCase))
            return BadRequest(new ApiResponse(new Error("unsupported.format", "Supported formats include PNG, JPEG and JPG ")));

        if (image.Length / 1024f / 1024f > 3)
            return BadRequest(new ApiResponse(new Error("maxsize.exceeded", "Maximum file limit is 3MB")));

        var tmp = image.FileName.Split(".");

        string? fileExt = tmp[tmp.Length - 1];

        var response = await Sender.Send(new UploadImageCommand(image, fileType, identifier));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }
}
