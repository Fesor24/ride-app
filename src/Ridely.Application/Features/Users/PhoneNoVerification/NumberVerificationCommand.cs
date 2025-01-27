using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Features.Accounts;

namespace Ridely.Application.Features.Users.PhoneNoVerification;
public sealed record NumberVerificationCommand(string PhoneNo, string Code, ApplicationInstance AppInstance) :
    ICommand<NumberVerificationResponse>;
