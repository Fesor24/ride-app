using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;

namespace Ridely.Application.Features.Riders.UpdateSavedLocation;
public sealed record UpdateSavedLocationCommand(long SavedLocationId, string Address,
    Location Coordinates) :
    ICommand;

public class UpdateSavedLocationValidator : AbstractValidator<UpdateSavedLocationCommand>
{
    public UpdateSavedLocationValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty().WithMessage("Address can not be empty")
            .NotNull().WithMessage("Address can not be null");

        RuleFor(x => x.Coordinates.Longitude)
            .GreaterThan(0).WithMessage("Longitude can not be 0");

        RuleFor(x => x.Coordinates.Latitude)
            .GreaterThan(0).WithMessage("Latitude can not be 0");
    }
}
