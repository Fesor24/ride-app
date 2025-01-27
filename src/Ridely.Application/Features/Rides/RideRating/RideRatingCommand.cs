using FluentValidation;
using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Rides.RideRating;
public sealed record RideRatingCommand(int RideId, int Rating, string Feedback) : ICommand;

public class RideRatingCommandValidator : AbstractValidator<RideRatingCommand>
{
    public RideRatingCommandValidator()
    {
        RuleFor(rating => rating.Feedback)
            .MaximumLength(200)
            .WithMessage("Feedback can not exceed 200 characters");

        RuleFor(rating => rating.Rating)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Acceptable values include 1-5")
            .LessThanOrEqualTo(5)
            .WithMessage("Acceptable values include 1-5");
    }
}
