using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Admin.Users.Query.GetProfile;
public sealed record GetProfileQuery(long UserId) : IQuery<GetProfileResponse>;
