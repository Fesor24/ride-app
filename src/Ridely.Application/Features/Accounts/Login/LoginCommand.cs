using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Accounts.Login;
public record LoginCommand(
    string Email,
    string Password
    ) : IRequest<Result<LoginResponse>>;
