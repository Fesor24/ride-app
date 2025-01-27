using FluentValidation;
using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Transactions.WithdrawFunds;
public sealed record WithdrawFundsCommand(long DriverId, int Amount,
    long BankAccountId, string Otp) : 
    ICommand;

public class WithdrawFundsValidator : AbstractValidator<WithdrawFundsCommand>
{
    public WithdrawFundsValidator()
    {
        RuleFor(x => x.Amount)
            .NotNull().WithMessage("Amount can not be null")
            .NotEmpty().WithMessage("Amount can not be empty")
            .GreaterThanOrEqualTo(200).WithMessage("Minimum amount is 200");
    }
}
