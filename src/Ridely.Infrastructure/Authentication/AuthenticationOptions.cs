namespace Soloride.Infrastructure.Authentication;
public sealed class AuthenticationOptions
{
    public string Audience { get; init; }
    public string Issuer { get; init; }
    public bool RequireHttpsMetaData { get; init; }
    public string MetadataUrl { get; init; }
    public string Secret { get; init; }
    public int ExpiryInHours { get; init; }
}
