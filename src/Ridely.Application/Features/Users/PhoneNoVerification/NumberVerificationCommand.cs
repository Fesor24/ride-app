using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Features.Accounts;

namespace Soloride.Application.Features.Users.PhoneNoVerification;
public sealed record NumberVerificationCommand(string PhoneNo, string Code, ApplicationInstance AppInstance) :
    ICommand<NumberVerificationResponse>;
