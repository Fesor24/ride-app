using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.Get;
public sealed record GetDriverQuery(long DriverId) : IQuery<GetDriverResponse>;
