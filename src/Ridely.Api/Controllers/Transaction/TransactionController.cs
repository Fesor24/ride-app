using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Transactions.AddPaymentCard;
using Ridely.Application.Features.Transactions.FundWallet;
using Ridely.Application.Features.Transactions.RemovePaymentCard;
using Ridely.Application.Features.Transactions.Verify;
using Ridely.Application.Features.Transactions.WithdrawFunds;
using Ridely.Domain.Abstractions;
using Ridely.Api.Controllers.Base;
using Ridely.Api.Extensions;
using Ridely.Api.Filter;
using Ridely.Api.Shared;

namespace Ridely.Api.Controllers.Transaction;

[ApiVersion(ApiVersions.V1)]
[ApiVersion(ApiVersions.V2)]
[Authorize]
[ResourceAuthorizationFilter]
[Route("api/v{v:apiVersion}/transaction")]
public class TransactionController : BaseController<TransactionController>
{
    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("payment-card/initiate")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<AddPaymentCardResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> AddPaymentCard()
    {
        var result = await Sender.Send(new AddPaymentCardCommand(RiderId));

        return result.Match(value => Ok(new ApiResponse<AddPaymentCardResponse>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpGet("payment/verify")]
    public async Task<IActionResult> Verify(string reference)
    {
        var result = await Sender.Send(new VerifyPaymentCommand(reference));

        return result.Match(value => Ok(value), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpDelete("payment-card/{paymentCardId}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> RemovePaymentCard(int paymentCardId)
    {
        var result = await Sender.Send(new RemovePaymentCardCommand(paymentCardId, RiderId));

        return result.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("withdraw")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<bool>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> Withdraw(WithdrawRequest request)
    {
        var result = await Sender.Send(new WithdrawFundsCommand(DriverId, request.Amount, 
            request.BankAccountId, request.Otp));

        return result.Match(value => Ok(new ApiResponse<bool>(value)), this.HandleErrorResult);
    }

    [MapToApiVersion(ApiVersions.V1)]
    [HttpPost("wallet/top-up")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<FundWalletResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> FundWallet(FundWalletRequest request)
    {
        long? driverId = GetDriverId();

        long? riderId = GetRiderId();

        var result = await Sender.Send(new FundWalletCommand(driverId, riderId, request.Amount));

        return result.Match(value => Ok(new ApiResponse<FundWalletResponse>(value)), this.HandleErrorResult);
    }

    

    //    [HttpPost("api/payment/charge")]
    //    public async Task<IActionResult> Charge(string authCode)
    //    {
    //        // use the email associated with the code
    //        var res = await _paymentService.ChargeAsync(authCode, "feso05@mail.com", 30);

    //        return Ok(res);

    //        //var result = await Sender.Send(new ChargeAuthorizationCommand());

    //        //return result.Match(value => Ok(value), this.HandleErrorResult);
    //    }
}
