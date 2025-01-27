using Ridely.Domain.Abstractions;
using Ridely.Domain.Shared;

namespace Ridely.Domain.Drivers;
public sealed class DriverReferrers : Entity
{
    private DriverReferrers()
    {
        
    }

    public DriverReferrers(long referredByDriverId, long referredUserId, ReferredUser referredUser)
    {
        DriverId = referredByDriverId;
        ReferredUserId = referredUserId;
        ReferredUser = referredUser;
    }

    public long DriverId { get; private set; }
    public Driver Driver { get; }
    public long ReferredUserId { get; private set; }
    public ReferredUser ReferredUser { get; private set; }
}
