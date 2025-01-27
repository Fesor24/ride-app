using MediatR;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Features.Accounts.Token;
public record GetAccessTokenCommand(string AccessToken, string RefreshToken) : 
    IRequest<Result<LoginResponse>>;
