using Ridely.Application.Features.Accounts;

namespace Ridely.Api.Controllers.User;

public sealed class InitiateVerificationRequest
{
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
}
