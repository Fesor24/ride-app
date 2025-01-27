using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Users;

namespace Ridely.Application.Abstractions.Authentication;
public interface IJwtService
{
    Task<(string AccessToken, string RefreshToken)> GenerateToken(User user, bool populateExp = false);
    Task<(string AccessToken, string RefreshToken)> GenerateToken(Rider rider, bool populateExp = false);
    Task<(string AccessToken, string RefreshToken)> GenerateToken(Driver driver, bool populateExp = false);
    Task<IDictionary<string, object>> GetClaimsFromToken(string token);
    Task<TValue?> GetClaimValueFromToken<TValue>(string key, string token);
    TValue? GetClaimValueFromToken<TValue>(string key, IDictionary<string, object> claims);
}
