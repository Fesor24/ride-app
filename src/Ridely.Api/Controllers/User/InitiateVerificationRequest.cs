using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Features.Accounts;

namespace RidelyAPI.Controllers.User;

public sealed class InitiateVerificationRequest
{
    public string PhoneNo { get; set; }
    public ApplicationInstance AppInstance { get; set; }
    public MessageMedium MessageMedium { get; set; } = MessageMedium.Whatsapp;
}
