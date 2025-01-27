using FluentValidation;
using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Common.Banks.Command.Create;
public record CreateBankCommand(
    string Name,
    string Code
    ) : IRequest<Result<bool>>;

public class CreateBankValidator : AbstractValidator<CreateBankCommand>
{
    public CreateBankValidator()
    {
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty().WithMessage("Name can not be null or empty");

        RuleFor(x => x.Code)
            .NotNull()
            .NotEmpty().WithMessage("Code can not be null or empty");
    }
}
