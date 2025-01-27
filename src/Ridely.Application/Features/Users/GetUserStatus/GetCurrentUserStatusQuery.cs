using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Users.GetUserStatus;
public sealed record GetCurrentUserStatusQuery(long? DriverId, long? RiderId) :
    IQuery<GetCurrentUserStatusResponse>;
