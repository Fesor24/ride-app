namespace Ridely.Domain.Rides;
public enum RideStatus
{
    Unknown = 0,
    FareEstimate = 1,
    Requested = 2,
    Matched = 3,
    Arrived = 4,
    Started = 5,
    Completed = 6,
    Reassigned = 7,
    Cancelled = 8
}
