using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Features.Accounts.Login;
public record LoginCommand(
    string Email,
    string Password
    ) : IRequest<Result<LoginResponse>>;
