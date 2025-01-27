using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soloride.Application.Features.Riders.AddSavedLocation;
using Soloride.Application.Features.Riders.DeleteSavedLocation;
using Soloride.Application.Features.Riders.Get;
using Soloride.Application.Features.Riders.GetSavedLocations;
using Soloride.Application.Features.Riders.RegisterRider;
using Soloride.Application.Features.Riders.UpdateSavedLocation;
using SolorideAPI.Controllers.Base;
using SolorideAPI.Extensions;
using SolorideAPI.Filter;
using SolorideAPI.Shared;

namespace SolorideAPI.Controllers.Rider;

[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Authorize]
[ResourceAuthorizationFilter]
[Route("api/v{v:apiVersion}/rider")]
public class RiderController : BaseController<RiderController>
{
    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(200, Type = typeof(ApiResponse<RegisterRiderResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> RegisterRider(RegisterRiderRequest request)
    {
        var response = await Sender.Send(new RegisterRiderCommand(
            request.FirstName, request.LastName, request.PhoneNo, request.Email,
            request.Gender, request.ReferrerCode));

        return response.Match(value => Ok(new ApiResponse<RegisterRiderResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetRiderResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetRider()
    {
        var response = await Sender.Send(new GetRiderQuery(RiderId));

        return response.Match(value => Ok(new ApiResponse<GetRiderResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("saved-location")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> AddSavedLocation(SavedLocationRequest request)
    {
        var response = await Sender.Send(new AddSavedLocationCommand(RiderId, request.LocationType,
            request.Coordinates, request.Address));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("saved-location")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpdateSavedLocation(UpdateSavedLocationRequest request)
    {
        var response = await Sender.Send(new UpdateSavedLocationCommand(request.SavedLocationId,
            request.Address, request.Coordinates));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpDelete("saved-location/{savedLocationId}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> DeleteSavedLocation(long savedLocationId)
    {
        var response = await Sender.Send(new DeleteSavedLocationCommand(savedLocationId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("saved-locations")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<List<GetSavedLocationResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetSavedLocation()
    {
        var response = await Sender.Send(new GetSavedLocationByRiderQuery(RiderId));

        return response.Match(value => Ok(new ApiResponse<List<GetSavedLocationResponse>>(value)),
            this.HandleErrorResult);
    }
}
