using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Users.Deactivate;
using Ridely.Application.Features.Users.Delete;
using Ridely.Application.Features.Users.GetUserStatus;
using Ridely.Application.Features.Users.InitiateEmailVerification;
using Ridely.Application.Features.Users.InitiatePhoneNoVerification;
using Ridely.Application.Features.Users.PhoneNoVerification;
using Ridely.Application.Features.Users.UpdateDeviceToken;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.User;

[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Authorize]
[ResourceAuthorizationFilter]
[Route("api/v{version:apiVersion}/user")]
public class UserController : BaseController<UserController>
{
    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("initiate-verification")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<InitiateNumberResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    [AllowAnonymous]
    public async Task<IActionResult> PhoneNoVerificationInit(InitiateVerificationRequest request)
    {
        var response = await Sender.Send(new InitiateNumberVerificationCommand(request.PhoneNo,
            request.AppInstance, request.MessageMedium));

        return response.Match(value => Ok(new ApiResponse<InitiateNumberResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("verify")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<NumberVerificationResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    [AllowAnonymous]
    public async Task<IActionResult> PhoneNoVerification(VerificationRequest request)
    {
        var response = await Sender.Send(new NumberVerificationCommand(request.PhoneNo,
            request.Code, request.AppInstance));

        return response.Match(value => Ok(new ApiResponse<NumberVerificationResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("deactivate")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> DeactivateUser()
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new DeactivateAccountCommand(driverId, riderId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpDelete]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> Delete()
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new DeleteAccountCommand(riderId, driverId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("current-status")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetCurrentUserStatusResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> CurrentUserStatus()
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new GetCurrentUserStatusQuery(driverId, riderId));

        return response.Match(value => Ok(new ApiResponse<GetCurrentUserStatusResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("device-token")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpdateDeviceToken(UpdateDeviceTokenRequest request)
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new UpdateDeviceTokenCommand(request.DeviceTokenId, riderId, driverId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("initiate-email-verification")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> InitiateEmailVerification()
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var response = await Sender.Send(new InitiateEmailVerificationCommand(driverId, riderId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }
}
