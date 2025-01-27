using FluentValidation;
using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Models;

namespace Soloride.Application.Features.Rides.GetChatMessages;
public sealed class GetChatMessagesQuery : IQuery<PaginatedList<GetChatMessagesResponse>>
{
    public long RideId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public long? RiderId { get; set; }
    public long? DriverId { get; set; }
}

public class GetRideChatMessageValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetRideChatMessageValidator()
    {
        RuleFor(x => x.RideId)
            .GreaterThan(0).WithMessage("Pass a valid ride id");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Pass a valid page number");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Pass a valid page size");
    }
}
