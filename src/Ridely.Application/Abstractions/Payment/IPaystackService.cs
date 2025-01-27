using Soloride.Application.Models.Payment;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Abstractions.Payment;
public interface IPaystackService
{
    Task<Result<InitializePaymentResponse>> InitializeAsync(InitializePayment initializePayment,
        bool cardPayment = false);
    Task<Result<VerifyPaymentResponse>> VerifyAsync(string transactionReference);
    Task<Result<ChargeAuthorizationPaymentResponse>> ChargeAsync(string authCode, string email,
        int amount, string reference);
    Task<Result<CreateRecipientPaymentResponse>> CreateRecipient(string accountName, string bankCode,
        string accountNo, string type = "nuban");
    Task<Result<InitiateTransferPaymentResponse>> InitiateTransfer(int amount,
        string recipientCode, string reference, string reason = "Driver withdrawal");

    Task<Result<InitiateTransferPaymentResponse>> FinalizeTransfer(string transferCode, string otp);

    Task<Result<ResolveAccountPaymentResponse>> ResolveAccount(string bankCode, string accountNo);

    Task<Result<RefundResponse>> Refund(string reference, int amount,
        string customerNote = "We apologize for the inconvenience", string merchantNote = "Unable to fulfill ride order");
}
