using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Admin.Driver.UpgradeCab;
using Ridely.Application.Features.Drivers.CreateBankAccount;
using Ridely.Application.Features.Drivers.DeleteBankAccount;
using Ridely.Application.Features.Drivers.Get;
using Ridely.Application.Features.Drivers.GetBankAcounts;
using Ridely.Application.Features.Drivers.GetOtp;
using Ridely.Application.Features.Drivers.RegisterDriver;
using Ridely.Application.Features.Drivers.Transactions;
using Ridely.Application.Features.Drivers.UpdateStatus;
using Ridely.Application.Features.Drivers.VerifyBankAccount;
using Ridely.Application.Models.Shared;
using Ridely.Domain.Models;
using Ridely.Shared.Constants;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Filter;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Driver;

[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Authorize(Roles = Roles.Driver)]
[ResourceAuthorizationFilter]
[Route("api/v{v:apiVersion}/driver")]
public class DriverController : BaseController<DriverController>
{
    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(200, Type = typeof(ApiResponse<RegisterDriverResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> RegisterDriver(RegisterDriverRequest request)
    {
        var response = await Sender.Send(new RegisterDriverCommand(
            request.ReferrerCode,
            new RegisterDriverInfo(request.Driver.FirstName,
            request.Driver.LastName, request.Driver.Gender,
            request.Driver.PhoneNo, request.Driver.Email,
            request.Driver.DriversLicenseNo, request.Driver.DriverService,
            request.Driver.ProfileImageBase64Url, request.Driver.DriversLicenseBase64Url,
            request.Driver.IdentityNo, request.Driver.IdentityType),
            new RegisterDriverVehicleInfo(request.Vehicle.Color, request.Vehicle.Year,
            request.Vehicle.Model, request.Vehicle.LicensePlateNo, request.Vehicle.Manufacturer, "")));

        return response.Match(value => Ok(new ApiResponse<RegisterDriverResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetDriverResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetDriver()
    {
        var response = await Sender.Send(new GetDriverQuery(DriverId));

        return response.Match(value => Ok(new ApiResponse<GetDriverResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("status")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpdateStatus(UpdateStatusRequest request)
    {
        var response = await Sender.Send(new UpdateStatusCommand(request.Status, DriverId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("otp/{reason}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetOtp(OtpReason reason)
    {
        var response = await Sender.Send(new GetOtpQuery(reason, DriverId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("bank-account")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> CreateBankAccout(UpdateBankAccountRequest request)
    {
        var response = await Sender.Send(new CreateBankAccountCommand(DriverId,
            request.BankId, request.AccountNo, request.Otp));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("bank-account/verify")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<VerifyBankAccountResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> VerifyBankAccount(string accountNo, int bankId)
    {
        var response = await Sender.Send(new VerifyBankAccountQuery(accountNo, bankId, DriverId));

        return response.Match(value => Ok(new ApiResponse<VerifyBankAccountResponse>(value)), this.HandleErrorResult);
    }

    [HttpGet("bank-accounts")]
    [MapToApiVersion(ApiVersions.V1)]
    [ProducesResponseType(200, Type = typeof(ApiResponse<List<GetBankAccountsResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetBankAccouts()
    {
        var response = await Sender.Send(new GetBankAccountsQuery(DriverId));

        return response.Match(value => Ok(new ApiResponse<List<GetBankAccountsResponse>>(value)), this.HandleErrorResult);
    }

    [HttpDelete("bank-account/{bankAccountId}")]
    [MapToApiVersion(ApiVersions.V1)]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetBankAccouts(int bankAccountId)
    {
        var response = await Sender.Send(new DeleteBankAccountCommand(bankAccountId, 
            DriverId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [HttpGet("transactions")]
    [MapToApiVersion(ApiVersions.V1)]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchDriverTransactionsResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchTransactions(int pageSize, int pageNumber)
    {
        var response = await Sender.Send(new SearchDriverTransactionsQuery
        {
            DriverId = DriverId,
            PageNumber = pageNumber,
            PageSize = pageSize
        });

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchDriverTransactionsResponse>>(value)),
            this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPut("cab")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> UpgradeCab(UpgradeCabRequest request)
    {
        var response = await Sender.Send(new UpgradeCabCommand(request.CabId));

        return response.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }
}
