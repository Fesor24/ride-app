using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Models.Payment;
using Ridely.Domain.Abstractions;

namespace Ridely.Infrastructure.Payments;
internal sealed class PaystackService : IPaystackService
{
    private readonly string _payStackSecret;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaystackService> _logger;

    public PaystackService(IConfiguration configuration, HttpClient httpClient, ILogger<PaystackService> logger)
    {
        _payStackSecret = Environment.GetEnvironmentVariable("PAYSTACK_SECRET") ??
        configuration["Paystack:Secret"] ?? throw new ArgumentNullException(nameof(_payStackSecret),
        "Paystack secret is null");

        string payStackBaseAddress = configuration["Paystack:BaseAddress"] ??
            throw new ArgumentNullException("Paystack_BaseAddress", "Paystack base address is null");

        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_payStackSecret}");
        _httpClient.BaseAddress = new Uri(payStackBaseAddress);
    }

    public async Task<Result<InitializePaymentResponse>> InitializeAsync(InitializePayment initializePayment,
        bool cardPayment = false)
    {
        int amount = initializePayment.Amount;

        if (initializePayment.CardVerification)
            amount = 50;

        string[] defaultChannels = ["card", "bank", "ussd", "bank_transfer"];

        string[] cardChannel = ["card"];

        var request = new
        {
            amount = amount * 100,
            email = initializePayment.Email,
            currency = "NGN",
            reference = initializePayment.Reference,
            //callback_url = $"{_configuration["Ridely:BaseUrl"]}/api/payment/verify",
            channels = cardPayment ? cardChannel : defaultChannels
        };

        var response = await _httpClient.PostAsync("/transaction/initialize", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var paymentResult = JsonSerializer.Deserialize<InitializePaymentResponse>(content);

            if (paymentResult is null) return Error.BadRequest("payment.deserialization", content);

            return paymentResult;
        }
        else
        {
            _logger.LogError("Payment Error: Details: {details}", await response.Content.ReadAsStringAsync());
            return Error.BadRequest("initiate.error", await response.Content.ReadAsStringAsync());
        }
    }

    public async Task<Result<VerifyPaymentResponse>> VerifyAsync(string transactionReference)
    {
        var response = await _httpClient.GetAsync($"/transaction/verify/{transactionReference}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<VerifyPaymentResponse>(content);

            if (result is null)
            {
                return Error.BadRequest("response.deserialization", content);
            }

            result.Data.AmountInNaira = result.Data.Amount / 100;

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest(response.ReasonPhrase ?? "verification.error",
                error?.Message ?? "Error during verification");
        }
    }

    public async Task<Result<ChargeAuthorizationPaymentResponse>> ChargeAsync(string authCode, string email, 
        int amount, string reference)
    {
        int amountToCharge = amount * 100;

        var request = new
        {
            email,
            amount = amountToCharge,
            authorization_code = authCode,
            reference
        };

        var response = await _httpClient.PostAsync("/transaction/charge_authorization", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ChargeAuthorizationPaymentResponse>(content);

            if (result is null) return Error.BadRequest("charge.deserialization", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest(response.ReasonPhrase ?? "charge.error", error?.Message ?? "Error occurred during charge");
        }
    }

    public async Task<Result<CreateRecipientPaymentResponse>> CreateRecipient(string accountName, string bankCode,
        string accountNo, string type = "nuban")
    {
        var request = new
        {
            type,
            name = accountName,
            account_number = accountNo,
            bank_code = bankCode,
            currency = "NGN"
        };

        var response = await _httpClient.PostAsync("/transferrecipient", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<CreateRecipientPaymentResponse>(content);

            if (result is null) return Error.BadRequest("recipient.deserialization", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest(response.ReasonPhrase ?? "recipient.error",
                error?.Message ?? "Error occurred during recipient creation");
        }
    }

    public async Task<Result<InitiateTransferPaymentResponse>> InitiateTransfer(int amount,
        string recipientCode, string reference, string reason = "Driver withdrawal")
    {
        amount *= 100;

        var request = new
        {
            source = "balance",
            reason,
            amount,
            recipient = recipientCode,
            reference
        };

        var response = await _httpClient.PostAsync("/transfer", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<InitiateTransferPaymentResponse>(content);

            if (result is null) return Error.BadRequest("transfer.deserialization", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest(response.ReasonPhrase ?? "initiatetransfer",
                error?.Message ?? "Error occurred during tranfer initialization");
        }
    }

    // If OTP is disabled, there is no need for this...
    public async Task<Result<InitiateTransferPaymentResponse>> FinalizeTransfer(string transferCode, string otp)
    {
        var request = new
        {
            transfer_code = transferCode,
            otp
        };

        var response = await _httpClient.PostAsync("/transfer/finalize_transfer", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<InitiateTransferPaymentResponse>(content);

            if (result is null) return Error.BadRequest("transfer.deserialization", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest(response.ReasonPhrase ?? "transfer.error",
                error?.Message ?? "Error occurred during finalizing transfer");
        }
    }

    public async Task<Result<ResolveAccountPaymentResponse>> ResolveAccount(string bankCode, string accountNo)
    {
        var response = await _httpClient.GetAsync($"/bank/resolve?account_number={accountNo}&bank_code={bankCode}");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<ResolveAccountPaymentResponse>(content);

            if (result is null) return Error.BadRequest("account.notresolved", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogError("An error occurred while resolving account. Message: {message}. Details: {details}",
                response.ReasonPhrase, content);

            var error = JsonSerializer.Deserialize<PaymentErrorResponse>(content);

            return Error.BadRequest("account.notresolved", "Account not resolved. Confirm your account number and try again");
        }
    }

    public async Task<Result<RefundResponse>> Refund(string reference, int amount, 
        string customerNote = "We apologize for the inconvenience", 
        string merchantNote = "Unable to fulfill ride order")
    {
        var request = new Dictionary<string, object>
        {
            {"transaction", reference },
            {"customer_note", customerNote },
            {"merchant_note", merchantNote },
            {"currency", "NGN" },
            {"amount",  amount * 100}
        };

        var response = await _httpClient.PostAsync($"/refund", JsonContent.Create(request));

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RefundResponse>(content);

            if (result is null) return Error.BadRequest("refund.deserialization", content);

            return result;
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogError("An error occurred while processing refund. Message: {message}. Details: {details}",
                response.ReasonPhrase, content);

            var error = JsonSerializer.Deserialize<RefundResponse>(content);

            return Error.BadRequest("refund.deserialization", response.ReasonPhrase);
        }
    }
}
