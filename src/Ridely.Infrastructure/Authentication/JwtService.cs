using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Users;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Infrastructure.Authentication;
internal sealed class JwtService : IJwtService
{
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly ApplicationDbContext _context;

    public JwtService(IOptions<AuthenticationOptions> authenticationOptions, ApplicationDbContext context)
    {
        _authenticationOptions = authenticationOptions.Value;
        _context = context;
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateToken(User user, bool populateExp = false)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.Secret));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JsonWebTokenHandler();

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Email] = user.Email,
            [ClaimTypes.Role] = user.RoleId.ToString(),
            [ClaimTypes.NameIdentifier] = user.Id.ToString()
        };

        string refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        if (populateExp)
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _context.Set<User>().Update(user);

        await _context.SaveChangesAsync();

        string accessToken = token.CreateToken(GetTokenDescriptor(signingCredentials, claims));

        return (accessToken, refreshToken);
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateToken(Rider rider, bool persistRefreshToken = true)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.Secret));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JsonWebTokenHandler();

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Email] = rider.Email,
            [ClaimTypes.NameIdentifier] = RiderKey.CustomNameIdentifier(rider.Id),
            [ClaimsConstant.Rider] = rider.Id.ToString(),
            [ClaimTypes.Role] = Roles.Rider
        };

        string accessToken = token.CreateToken(GetTokenDescriptor(signingCredentials, claims));

        if (persistRefreshToken)
        {
            string refreshToken = GenerateRefreshToken();

            rider.UpdateRefreshToken(refreshToken);

            _context.Set<Rider>().Update(rider);

            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);

        }

        return (accessToken, string.Empty);
    }

    public async Task<(string AccessToken, string RefreshToken)> GenerateToken(Driver driver, bool persistRefreshToken = true)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.Secret));

        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JsonWebTokenHandler();

        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Email] = driver.Email,
            [ClaimTypes.NameIdentifier] = DriverKey.CustomNameIdentifier(driver.Id),
            [ClaimsConstant.Driver] = driver.Id.ToString(),
            [ClaimTypes.Role] = Roles.Driver
        };

        string accessToken = token.CreateToken(GetTokenDescriptor(signingCredentials, claims));

        if (persistRefreshToken)
        {
            string refreshToken = GenerateRefreshToken();

            driver.UpdateRefreshToken(refreshToken);

            _context.Set<Driver>().Update(driver);

            await _context.SaveChangesAsync();

            return (accessToken, refreshToken);
        }

        return (accessToken, string.Empty);
    }

    private string GenerateRefreshToken()
    {
        byte[] byteArr = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(byteArr);

        return Convert.ToBase64String(byteArr);
    }

    private SecurityTokenDescriptor GetTokenDescriptor(SigningCredentials creds, IDictionary<string, object> claims) =>
        new()
        {
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddHours(_authenticationOptions.ExpiryInHours),
            SigningCredentials = creds,
            Claims = claims,
            Issuer = _authenticationOptions.Issuer,
            NotBefore = DateTime.UtcNow,
            Audience = _authenticationOptions.Audience
        };

    public async Task<IDictionary<string, object>> GetClaimsFromToken(string token)
    {
        if (token.StartsWith("Bearer "))
            token = token.Replace("Bearer ", "");

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.Secret)),
            ValidIssuer = _authenticationOptions.Issuer,
            ValidAudience = _authenticationOptions.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();

        var validationResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

        if (!validationResult.IsValid) return null;

        var jtoken = validationResult.SecurityToken as JsonWebToken;

        if (jtoken == null || !jtoken.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return validationResult.Claims;
    }

    public async Task<TValue?> GetClaimValueFromToken<TValue>(string key, string token)
    {
        var claims = await GetClaimsFromToken(token);

        bool valueExist = claims.TryGetValue(key, out var value);

        if (!valueExist || value == null) return default;

        return (TValue)value;
    }

    public TValue? GetClaimValueFromToken<TValue>(string key, IDictionary<string, object> claims)
    {
        bool valueExist = claims.TryGetValue(key, out var value);

        if (!valueExist || value == null) return default;

        return (TValue)value;
    }
}
