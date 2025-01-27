using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Webhook.PaystackWebhook;
public sealed class PaystackWebhookCommand : ICommand
{
    public string Event { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
    public long Id { get; set; }
    public string AuthorizationCode { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Last4 { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public object RawData { get; set; }
}
