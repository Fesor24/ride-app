using FluentValidation;
using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Riders.DeleteSavedLocation;
public sealed record DeleteSavedLocationCommand(long SavedLocationId) :
    ICommand;

public class DeleteSavedLocationValidator : AbstractValidator<DeleteSavedLocationCommand>
{
    public DeleteSavedLocationValidator()
    {
        RuleFor(x => x.SavedLocationId)
            .NotNull().WithMessage("Location Id can not be null")
            .GreaterThan(0).WithMessage("Pass a valid location id");
    }
}
