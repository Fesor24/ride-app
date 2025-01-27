using System.Text.Json.Serialization;

namespace Soloride.Application.Models.Payment;

public abstract class BasePaymentResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
public abstract class BasePaymentResponse<TData>
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("data")]
    public TData Data { get; set; }
}

public class InitializePaymentResponse : BasePaymentResponse<InitializePaymentResponseData> { }

public class VerifyPaymentResponse : BasePaymentResponse<VerifyPaymentResponseData>
{
  
}

public class ChargeAuthorizationPaymentResponse : BasePaymentResponse<ChargeAuthorizationResponseData>
{
}

public class CreateRecipientPaymentResponse : BasePaymentResponse<CreateRecipientResponseData> { }

public class InitiateTransferPaymentResponse : BasePaymentResponse<InitiateTransferResponseData> { }

public class ResolveAccountPaymentResponse : BasePaymentResponse<ResolveAccountResponseData> { }

public class RefundResponse : BasePaymentResponse<RefundResponseData> { }

public class PaymentErrorResponse
{
    [JsonPropertyName("status")]
    public bool Status { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; }
}

public class InitializePaymentResponseData
{
    [JsonPropertyName("authorization_url")]
    public string AuthorizationUrl { get; set; }
    [JsonPropertyName("access_code")]
    public string AccessCode { get; set; }
    [JsonPropertyName("reference")]
    public string Reference { get; set; }
}

public class VerifyPaymentResponseData
{
    [JsonPropertyName("reference")]
    public string Reference { get; set; }
    [JsonPropertyName("amount")]
    public int Amount { get; set; } // paystack returns amount in kobo
    public int AmountInNaira { get; set; }
    [JsonPropertyName("paid_at")]
    public DateTime PaidAt { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }// if success
    [JsonPropertyName("fees")]
    public int Fees { get; set; }
    [JsonPropertyName("authorization")]
    public VerifyPaymentResponseDataAuth Authorization { get; set; }

    public class VerifyPaymentResponseDataAuth
    {
        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; }
        [JsonPropertyName("last4")]
        public string Last4 { get; set; }
        [JsonPropertyName("exp_month")]
        public string ExpiryMonth { get; set; }
        [JsonPropertyName("exp_year")]
        public string ExpiryYear { get; set; }
        [JsonPropertyName("card_type")]
        public string CardType { get; set; }// visa,
        [JsonPropertyName("bank")]
        public string Bank { get; set; }
        [JsonPropertyName("reusable")]
        public bool Reusable { get; set; }// if false, we can not charge card subsequently
        [JsonPropertyName("signature")]
        public string Signature { get; set; }
    }

    public class VerifyPaymentCustomerResponseData
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}

public class ChargeAuthorizationResponseData
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    [JsonPropertyName("transaction_date")]
    public DateTime TransactionDate { get; set; }
    [JsonPropertyName("reference")]
    public string Reference { get; set; }

}

public class CreateRecipientResponseData
{
    [JsonPropertyName("recipient_code")]
    public string RecipientCode { get; set; }
}

public class InitiateTransferResponseData
{
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    [JsonPropertyName("reference")]
    public string Reference { get; set; }
    [JsonPropertyName("transfer_code")]
    public string TransferCode { get; set; }
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; } // could be pending or otp if otp is required...mostly be pending as withdrawals might not be processed immediately
}

public class ResolveAccountResponseData
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; }
    [JsonPropertyName("account_name")]
    public string AccountName { get; set; }
    [JsonPropertyName("bank_id")]
    public int BankId { get; set; }
}

public class RefundResponseData
{
    [JsonPropertyName("transaction")]
    public TransactionData Transaction { get; set; }
    [JsonPropertyName("merchant_note")]
    public string MerchantNote { get; set; }
    [JsonPropertyName("customer_note")]
    public string CustomerNote { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("refunded_by")]
    public string RefundedBy { get; set; }
    [JsonPropertyName("expected_at")]
    public string ExpectedAt { get; set; }
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; }
    public class TransactionData
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        [JsonPropertyName("reference")]
        public string Reference { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        [JsonPropertyName("paid_at")]
        public string PaidAt { get; set; }
        [JsonPropertyName("channel")]
        public string Channel { get; set; }
    }
}
