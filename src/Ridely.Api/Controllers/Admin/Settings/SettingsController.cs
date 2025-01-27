using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soloride.Application.Features.Admin.Settings.Command.UpdateRideSettings;
using Soloride.Application.Features.Admin.Settings.Query.GetSettings;
using Soloride.Application.Features.Common.Banks.Command.CreateMany;
using SolorideAPI.Controllers.Base;
using SolorideAPI.Extensions;
using SolorideAPI.Shared;

namespace SolorideAPI.Controllers.Admin.Settings;

[Route("api/settings")]
public class SettingsController : AdminBaseController<SettingsController>
{
    [HttpPut("ride")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpdateRideSettings(RideSettingsRequest rideSettings)
    {
        var response = await Sender.Send(new UpdateRideSettingsCommand(
            rideSettings.BaseFare,
            rideSettings.RatePerKilometer,
            rideSettings.DeliveryRatePerKilometer,
            rideSettings.DriverCommission,
            rideSettings.RatePerMinute,
            rideSettings.PremiumCab
            ));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetSettingsResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetSettings()
    {
        var response = await Sender.Send(new GetSettingsQuery());

        return response.Match(value => Ok(new ApiResponse<GetSettingsResponse>(value)), this.HandleErrorResult);
    }

    [HttpPost("banks")]
    [AllowAnonymous]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetSettingsResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> AddBanks(BankRequest request)
    {
        var response = await Sender.Send(new CreateBanksCommand
        {
            UserId = 1,
            Banks = request.Banks.ConvertAll(bank => new CreateBank
            {
                Type = bank.Type,
                Name = bank.Name,
                Code = bank.Code,
            })
        });

        return Ok();
    }
}
