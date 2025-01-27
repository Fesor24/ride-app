using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Users.GetUserStatus;
public sealed record GetCurrentUserStatusQuery(long? DriverId, long? RiderId) :
    IQuery<GetCurrentUserStatusResponse>;
