using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ridely.Domain.Rides;
public sealed class Chat : Entity
{
    private Chat()
    {
        
    }

    public Chat(long rideId, UserType sender, UserType recipient,
        string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ApplicationException("Message can not be empty");

        if (message.Length > 200)
            throw new ApplicationException("Message exceeds maximum length (200)");

        CreatedAtUtc = DateTime.UtcNow;
        IsRead = false;
        RideId = rideId;
        Sender = sender;
        Recipient = recipient;
        Message = message;
    }

    public long RideId { get; private set; }
    [ForeignKey(nameof(RideId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Ride Ride { get; }
    public UserType Sender { get; private set; }
    public UserType Recipient { get; private set; }
    public string Message { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void SetParties(UserType sender, UserType recipient)
    {
        Sender = sender;
        Recipient = recipient;
    }
}
