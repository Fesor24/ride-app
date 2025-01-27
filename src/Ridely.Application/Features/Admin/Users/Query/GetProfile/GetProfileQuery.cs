using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Users.Query.GetProfile;
public sealed record GetProfileQuery(long UserId) : IQuery<GetProfileResponse>;
