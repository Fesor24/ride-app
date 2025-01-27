using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soloride.Application.Features.VoiceCall.AccessToken;
using Soloride.Application.Features.VoiceCall.EndCall;
using Soloride.Application.Features.VoiceCall.NotifyRecipient;
using Soloride.Application.Features.VoiceCall.StartCall;
using SolorideAPI.Controllers.Base;
using SolorideAPI.Extensions;
using SolorideAPI.Shared;

namespace SolorideAPI.Controllers.VoiceCall
{
    [ApiVersion(ApiVersions.V1)]
    [ApiVersion(ApiVersions.V2)]
    [ApiController]
    [Authorize]
    [Route("api/v{version:apiVersion}/voice-call")]
    public class VoiceCallController : BaseController<VoiceCallController>
    {
        [MapToApiVersion(ApiVersions.V1)]
        [HttpGet("token/{rideId}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<GetVoiceAccessTokenResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetVoiceToken(int rideId)
        {
            long? driverId = GetDriverId();

            long? riderId = GetRiderId();

            var response = await Sender.Send(new GetVoiceAccessTokenQuery(rideId, riderId, driverId));

            return response.Match(value => Ok(new ApiResponse<GetVoiceAccessTokenResponse>(value)), this.HandleErrorResult);
        }

        [MapToApiVersion(ApiVersions.V1)]
        [HttpPost("notify")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(400, Type = typeof(ApiResponse))]
        public async Task<IActionResult> NotifyRecipient(NotifyCallRecipientRequest request)
        {
            long? driverId = GetDriverId();

            long? riderId = GetRiderId();

            var response = await Sender.Send(new NotifyRecipientCommand(request.RideId, driverId, riderId));

            return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
        }

        [MapToApiVersion(ApiVersions.V1)]
        [HttpPost("start")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<StartCallResponse>))]
        [ProducesResponseType(400, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StartCall(StartCallRequest request)
        {
            bool driverCalled = request.Caller == CallUserRequest.Driver;

            var response = await Sender.Send(new StartCallCommand(request.RideId, driverCalled));

            return response.Match(value => Ok(new ApiResponse<StartCallResponse>(value)), this.HandleErrorResult);
        }

        [MapToApiVersion(ApiVersions.V1)]
        [HttpPut("end")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
        [ProducesResponseType(400, Type = typeof(ApiResponse))]
        public async Task<IActionResult> EndCall(EndCallRequest request)
        {
            var response = await Sender.Send(new EndCallCommand(request.CallId));

            return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
        }
    }
}
