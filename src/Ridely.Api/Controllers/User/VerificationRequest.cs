using Soloride.Application.Features.Accounts;

namespace SolorideAPI.Controllers.User;

public sealed class VerificationRequest
{
    public string Code { get; set; }
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
}
