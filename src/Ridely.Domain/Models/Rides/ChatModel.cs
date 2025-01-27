namespace Soloride.Domain.Models.Rides;
public class ChatModel : BaseModel
{
    public string Sender { get; set; }
    public string SenderName { get; set; }
    public string Recipient { get; set; }
    public string RecipientName { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}
