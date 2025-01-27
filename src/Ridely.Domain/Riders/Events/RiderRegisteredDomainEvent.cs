using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Riders.Events;
public sealed record RiderRegisteredDomainEvent(
    string PhoneNo,
    string ReferredByCode
    ) : IDomainEvent;
