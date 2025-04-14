using FluentValidation;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Models.Shared;

namespace Ridely.Application.Features.Rides.GetFareEstimate;
public sealed record GetFareEstimateCommand(
    LocationRequest Source,
    LocationRequest Destination,
    LocationRequest? Waypoint,
    string SourceAddress,
    string DestinationAddress,
    string? WayPointAddress,
    long RiderId) :
    ICommand<GetFareEstimateResponse>;

public class GetFareEstimateValidator : AbstractValidator<GetFareEstimateCommand>
{
    public GetFareEstimateValidator()
    {
        RuleFor(x => x.SourceAddress)
            .NotEmpty()
            .NotNull()
            .WithMessage("Specify an address source");

        RuleFor(x => x.DestinationAddress)
           .NotEmpty()
           .NotNull()
           .WithMessage("Specify an address destination");
    }
}
