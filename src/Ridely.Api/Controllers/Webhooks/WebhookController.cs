using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Webhook.PaystackWebhook;
using RidelyAPI.Extensions;

namespace RidelyAPI.Controllers.Webhooks
{
    [Route("api/webhook")]
    [AllowAnonymous]
    [ApiController]
    public class WebhookController(ISender Sender) : ControllerBase
    {
        [HttpPost("paystack")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook(PaymentWebhookRequest request)
        {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var result = await Sender.Send(new PaystackWebhookCommand
            {
                Amount = request.Data.Amount,
                Event = request.Event,
                Reference = !string.IsNullOrWhiteSpace(request.Data.TransactionReference) ? 
                    request.Data.TransactionReference : 
                    request.Data.Reference,
                Id = request.Data.Id,
                Status = request.Data.Status,
                Last4 = request.Data.Authorization?.Last4 ?? "",
                ExpiryMonth = request.Data.Authorization?.ExpiryMonth ?? "",
                ExpiryYear = request.Data.Authorization?.ExpiryYear ?? "",
                AuthorizationCode = request.Data.Authorization?.AuthorizationCode ?? "",
                Bank = request.Data.Authorization?.Bank ?? "",
                CardType = request.Data.Authorization?.CardType ?? "",
                RawData = request,
                CustomerEmail = request.Data.Customer?.Email ?? "",
                Signature = request.Data.Authorization?.Signature ?? ""
            });

            return result.Match(value => Ok(value), this.HandleErrorResult);
        }
    }
}
