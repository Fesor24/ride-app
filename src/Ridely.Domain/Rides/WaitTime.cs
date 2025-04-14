using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Rides;
public sealed class WaitTime : Entity
{
    public long Amount { get; private set; }
    public int Minute { get; private set; }
}
