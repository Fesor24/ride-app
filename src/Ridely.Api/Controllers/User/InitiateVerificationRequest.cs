using Ridely.Application.Features.Accounts;

namespace RidelyAPI.Controllers.User;

public sealed class InitiateVerificationRequest
{
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
}
