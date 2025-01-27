using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Features.Accounts;

namespace Ridely.Application.Features.Users.InitiatePhoneNoVerification;
public sealed record InitiateNumberVerificationCommand(string PhoneNo, ApplicationInstance AppInstance) :
    ICommand<InitiateNumberResponse>;

public class InitiatePhoneNumberVerificationValidator : AbstractValidator<InitiateNumberVerificationCommand>
{
    public InitiatePhoneNumberVerificationValidator()
    {
        RuleFor(x => x.PhoneNo)
            .NotNull().WithMessage("Phone number can not be null")
            .NotEmpty().WithMessage("Phone number can not be empty");
    }
}
