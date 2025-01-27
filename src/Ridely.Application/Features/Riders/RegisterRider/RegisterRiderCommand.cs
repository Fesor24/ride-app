using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Shared;

namespace Ridely.Application.Features.Riders.RegisterRider;
public sealed record RegisterRiderCommand(
    string FirstName,
    string LastName,
    string PhoneNo,
    string Email,
    Gender Gender,
    string? ReferrerCode
    ) : ICommand<RegisterRiderResponse>;

public class RegisterRiderCommandValidator : AbstractValidator<RegisterRiderCommand>
{
    public RegisterRiderCommandValidator()
    {
        RuleFor(x => x.PhoneNo)
            .NotEmpty().WithMessage("Phone no can not be empty")
            .NotNull().WithMessage("Phone no can not be null")
            .MaximumLength(15).WithMessage("Phone no can not be more than 15 characters")
            .MinimumLength(10).WithMessage("Phone no must be at least 10 characters");

        RuleFor(x => x.FirstName)
           .NotEmpty().WithMessage("First name can not be empty")
           .NotNull().WithMessage("First name can not be null")
           .MaximumLength(45).WithMessage("First name can not exceed 45 characters");

        RuleFor(x => x.LastName)
           .NotEmpty().WithMessage("Last name can not be empty")
           .NotNull().WithMessage("Last name can not be null")
           .MaximumLength(45).WithMessage("Last name can not exceed 45 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email can not be empty")
            .NotNull().WithMessage("Email can not be null")
            .MaximumLength(45).WithMessage("Email can not exceed 45 characters")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.FirstName)
           .NotEmpty().WithMessage("First name can not be empty")
           .NotNull().WithMessage("First name can not be null")
           .MaximumLength(45).WithMessage("First name can not exceed 45 characters");
    }
}
