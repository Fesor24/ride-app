using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Drivers.Events;
public sealed record DriverRegisteredDomainEvent(
    string PhoneNo,
    string RefferedBy,
    string ProfileImageBase64,
    string DriversLicenseBase64
    ) : IDomainEvent;
