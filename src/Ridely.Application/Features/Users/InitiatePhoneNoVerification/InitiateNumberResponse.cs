namespace Soloride.Application.Features.Users.InitiatePhoneNoVerification;
public sealed class InitiateNumberResponse
{
    public string Code { get; set; }
    public string CodeExpiry { get; set; }
    public int CodeExpiryInSeconds { get; set; }
    public bool NewUser { get; set; }
}
