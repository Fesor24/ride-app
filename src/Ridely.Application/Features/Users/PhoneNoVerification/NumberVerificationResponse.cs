namespace Ridely.Application.Features.Users.PhoneNoVerification;
public sealed class NumberVerificationResponse
{
    public bool CodeVerificationStatus { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public bool NewUser { get; set; }
}
