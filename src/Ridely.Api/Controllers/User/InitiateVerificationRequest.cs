using Soloride.Application.Features.Accounts;

namespace SolorideAPI.Controllers.User;

public sealed class InitiateVerificationRequest
{
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
}
