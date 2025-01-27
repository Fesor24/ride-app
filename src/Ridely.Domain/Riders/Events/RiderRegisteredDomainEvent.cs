using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Riders.Events;
public sealed record RiderRegisteredDomainEvent(
    string PhoneNo,
    string ReferredByCode
    ) : IDomainEvent;
