using FluentValidation;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Models.Shared;

namespace Soloride.Application.Features.Rides.GetFareEstimate;
public sealed record GetFareEstimateCommand(
    LocationRequest Source,
    LocationRequest Destination,
    string SourceAddress,
    string DestinationAddress,
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
