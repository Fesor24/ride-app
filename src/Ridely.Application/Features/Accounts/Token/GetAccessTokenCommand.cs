using MediatR;
using Soloride.Application.Features.Accounts.Login;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Accounts.Token;
public record GetAccessTokenCommand(string AccessToken, string RefreshToken) : 
    IRequest<Result<LoginResponse>>;
