using System.Text.Json.Serialization;

namespace SolorideAPI.Controllers.Webhooks;

public class PaymentWebhookRequest
{
    //events to handle...charge.success (after successful charge), transfer.success (after successful withdrawal), transfer.failed, transfer.reversed
    // paymentrequest.success
    public string Event { get; set; }
    public PaymentWebhookData Data { get; set; }
}

public class PaymentWebhookData
{
    // confirm if works...amount for refund request seems to be a string...
    public int Amount { get; set; }
    public long Id { get; set; }
    public string Reference { get; set; }
    public string Status { get; set; } //always check if this is success

    // used by refund event
    [JsonPropertyName("transaction_reference")]
    public string? TransactionReference { get; set; }
    public string? RefundReference { get; set; }
    public CardAuthorization? Authorization { get; set; }
    public Customer? Customer { get; set; }
}

public class CardAuthorization
{
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
    public string Last4 { get; set; }
    [JsonPropertyName("exp_month")]
    public string ExpiryMonth { get; set; }
    [JsonPropertyName("exp_year")]
    public string ExpiryYear { get; set; }
    public string Channel { get; set; }
    [JsonPropertyName("card_type")]
    public string CardType { get; set; }
    public string Bank { get; set; }
    public bool Reusable { get; set; }
    public string Signature { get; set; }
}

// for refund events
public class Customer
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set;}
    [JsonPropertyName("last_name")]
    public string? LastName { get; set;}
    [JsonPropertyName("email")]
    public string Email { get; set; }
}
