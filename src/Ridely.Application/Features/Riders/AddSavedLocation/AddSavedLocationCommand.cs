using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Models;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.AddSavedLocation;
public sealed record AddSavedLocationCommand(long RiderId,
    SavedLocationType LocationType,
    Location Coordinates, string Address) :
    ICommand;

public class AddSavedLocationValidator : AbstractValidator<AddSavedLocationCommand>
{
    public AddSavedLocationValidator()
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
