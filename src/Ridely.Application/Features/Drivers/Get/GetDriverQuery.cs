using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.Get;
public sealed record GetDriverQuery(long DriverId) : IQuery<GetDriverResponse>;
