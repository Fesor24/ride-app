using Soloride.Domain.Abstractions;
using Soloride.Domain.Shared;

namespace Soloride.Domain.Riders;
public sealed class RiderReferrers : Entity
{
    private RiderReferrers()
    {
        
    }

    public RiderReferrers(long referredByRiderId, long referredUserId, ReferredUser referredUser)
    {
        RiderId = referredByRiderId;
        ReferredUserId = referredUserId;
        ReferredUser = referredUser;
    }

    public long RiderId { get; set; }
    public Rider Rider { get; }
    public long ReferredUserId { get; private set; }
    public ReferredUser ReferredUser { get; private set; }
}
