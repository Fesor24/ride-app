using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public sealed class RideLog : Entity
{
    private RideLog()
    {
        
    }

    public RideLog(long rideId, RideLogEvent @event)
    {
        RideId = rideId;
        Event = @event;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long RideId { get; private set; }
    [ForeignKey(nameof(RideId))]
    public Ride Ride { get; private set; }
    public string? Details { get; private set; }
    public RideLogEvent Event { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public void SetDetails(WaitTimeRequestedEventDetails details)
    {
        if (Event != RideLogEvent.WaitTimeRequested) return;

        Details = JsonSerializer.Serialize(details);
    }
}

public sealed record WaitTimeRequestedEventDetails(long WaitTimeId, long Amount, int TimeInMinutes, DateTime AcceptedAtUtc);
