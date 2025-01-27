using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Users;

namespace Soloride.Application.Features.Admin.Users.Query.GetProfile;
internal class GetProfileQueryHandler(IUserRepository userRepository) : IRequestHandler<GetProfileQuery, Result<GetProfileResponse>>
{
    public async Task<Result<GetProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository
            .GetAsync(request.UserId);

        if (user is null) return Error.NotFound("USER_NOTFOUND", "User not found");

        return new GetProfileResponse
        {
            Id = user.Id,
            Role = user.Role.Code
        };
    }
}
