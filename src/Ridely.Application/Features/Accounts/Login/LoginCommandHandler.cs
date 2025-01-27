using MediatR;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Security;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Users;

namespace Ridely.Application.Features.Accounts.Login;
internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IJwtService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;

    public LoginCommandHandler(IJwtService tokenService, IUserRepository userRepository,
        IHashService hashService)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _hashService = hashService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository
            .GetByEmailAsync(request.Email.ToLower().Trim());

        if (user is null) return Error.Unauthorized("unauthotized", "Invalid credentials");

        if (string.IsNullOrWhiteSpace(user.Password))
            return Error.Unauthorized("unauthorized", "Invalid credentials");

        bool comparePassword = _hashService.ComparePassword(user.Password, request.Password);

        if(!comparePassword) return Error.Unauthorized("UNAUTHORIZED", "Invalid credentials");

        (string accessToken, string refreshToken) = await _tokenService.GenerateToken(user);

        return new LoginResponse(accessToken, refreshToken);
    }
}
