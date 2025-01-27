using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Users;

namespace Soloride.Application.Abstractions.Authentication;
public interface IJwtService
{
    Task<(string AccessToken, string RefreshToken)> GenerateToken(User user, bool populateExp = false);
    Task<(string AccessToken, string RefreshToken)> GenerateToken(Rider rider, bool populateExp = false);
    Task<(string AccessToken, string RefreshToken)> GenerateToken(Driver driver, bool populateExp = false);
    Task<IDictionary<string, object>> GetClaimsFromToken(string token);
    Task<TValue?> GetClaimValueFromToken<TValue>(string key, string token);
    TValue? GetClaimValueFromToken<TValue>(string key, IDictionary<string, object> claims);
}
