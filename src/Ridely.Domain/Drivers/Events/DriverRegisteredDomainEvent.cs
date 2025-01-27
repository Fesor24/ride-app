using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Drivers.Events;
public sealed record DriverRegisteredDomainEvent(
    string PhoneNo,
    string RefferedBy,
    string ProfileImageBase64,
    string DriversLicenseBase64
    ) : IDomainEvent;
