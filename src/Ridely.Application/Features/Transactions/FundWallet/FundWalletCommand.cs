using FluentValidation;
using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Transactions.FundWallet;
public sealed record FundWalletCommand(long? DriverId, long? RiderId, int Amount) :
    ICommand<FundWalletResponse>;

public class FundWalletValidator : AbstractValidator<FundWalletCommand>
{
    public FundWalletValidator()
    {
        RuleFor(x => x.Amount)
            .NotNull().WithMessage("Amount can not be null")
            .NotEmpty().WithMessage("Amount can not be empty")
            .GreaterThanOrEqualTo(100).WithMessage("Minimum amount is 100");
    }
}
