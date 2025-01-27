using System.ComponentModel.DataAnnotations.Schema;
using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public sealed class RideLog : Entity
{
    private RideLog()
    {
        
    }

    public RideLog(long rideId, RideStatus status)
    {
        RideId = rideId;
        Status = status;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long RideId { get; private set; }
    [ForeignKey(nameof(RideId))]
    public Ride Ride { get; }
    public RideStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
