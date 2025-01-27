using MediatR;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Features.Accounts.Login;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Shared.Constants;

namespace Ridely.Application.Features.Accounts.Token;
internal sealed class GetAccessTokenCommandHandler(IJwtService tokenService, IRiderRepository riderRepository,
    IDriverRepository driverRepository) : 
    IRequestHandler<GetAccessTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(GetAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var claims = await tokenService.GetClaimsFromToken(request.AccessToken);

        if (claims is null) return Error.BadRequest("invalid.token", "Token is invalid");

        bool isRider = false;

        if (claims.ContainsKey(ClaimsConstant.Rider))
            isRider = true;

        if (isRider)
        {
            string? identifier = tokenService.GetClaimValueFromToken<string>(ClaimsConstant.Rider, claims);

            if (string.IsNullOrWhiteSpace(identifier)) return Error.BadRequest("bad.request", "No identifier in token");

            bool parsed = int.TryParse(identifier, out int riderId);

            if(!parsed) return Error.BadRequest("bad.request", "Identifier in token type mismatch");

            var rider = await riderRepository.GetAsync(riderId);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            if (rider.RefreshToken != request.RefreshToken)
                return Error.BadRequest("invalid.token", "Refresh token is invalid");

            if (rider.RefreshTokenExpiry <= DateTime.UtcNow)
                return Error.BadRequest("refreshtoken.expired", "Refresh token has expired");

            (string AccessToken, string RefreshToken) =  await tokenService
                .GenerateToken(rider, false);

            return new LoginResponse(AccessToken, RefreshToken);
        }
        else
        {
            string? identifier = tokenService.GetClaimValueFromToken<string>(ClaimsConstant.Driver, claims);

            if (string.IsNullOrWhiteSpace(identifier)) return Error.BadRequest("bad.request", "No identifier in token");

            bool parsed = int.TryParse(identifier, out int driverId);

            if (!parsed) return Error.BadRequest("bad.request", "Identifier in token type mismatch");

            var driver = await driverRepository.GetAsync(driverId);

            if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

            if (driver.RefreshToken != request.RefreshToken)
                return Error.BadRequest("refreshtoken.invalid", "Refresh token is invalid");

            if (driver.RefreshTokenExpiry <= DateTime.UtcNow)
                return Error.BadRequest("refreshtoken.expiry", "Refresh token has expired");

            (string AccessToken, string RefreshToken) = await tokenService
                .GenerateToken(driver, populateExp: false);

            return new LoginResponse(AccessToken, RefreshToken);
        }
    }
}
