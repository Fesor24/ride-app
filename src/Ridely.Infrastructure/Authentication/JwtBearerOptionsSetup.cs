using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Soloride.Infrastructure.Authentication;
internal sealed class JwtBearerOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;
    public JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    {
        _authenticationOptions = authenticationOptions.Value;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }

    public void Configure(JwtBearerOptions options)
    {
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetaData;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = _authenticationOptions.Audience,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _authenticationOptions.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationOptions.Secret)),
            ValidateLifetime = true
        };
    }
}
