namespace Soloride.Application.Features.Rides.GetChatMessages;
public sealed class GetChatMessagesResponse
{
    public long Id { get; set; }
    public string Sender { get; set; }
    public string SenderName { get; set; }
    public string Recipient { get; set; }
    public string RecipientName { get; set; }
    public string Message { get; set; }
    public string CreatedAt { get; set; }
}
