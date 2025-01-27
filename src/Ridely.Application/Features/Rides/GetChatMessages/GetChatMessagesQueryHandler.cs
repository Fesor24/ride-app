using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.GetChatMessages;
internal sealed class GetChatMessagesQueryHandler(IChatRepository chatRepository) :
    IQueryHandler<GetChatMessagesQuery, PaginatedList<GetChatMessagesResponse>>
{
    public async Task<Result<PaginatedList<GetChatMessagesResponse>>> Handle(GetChatMessagesQuery request,
        CancellationToken cancellationToken)
    {
        var searchParams = new ChatSearchQueryParams
        {
            RideId = request.RideId,
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            RiderId = request.RiderId,
            DriverId = request.DriverId
        };

        var res = await chatRepository
            .SearchAsync(searchParams);

        return new PaginatedList<GetChatMessagesResponse>
        {
            Items = res.Items.ConvertAll(chat => new GetChatMessagesResponse
            {
                CreatedAt = chat.CreatedAt.ToCustomDateString(),
                Id = chat.Id,
                Message = chat.Message,
                Recipient = chat.Recipient,
                RecipientName = chat.RecipientName,
                Sender = chat.Sender,
                SenderName = chat.SenderName,
            }),
            PageNumber = res.PageNumber,
            PageSize = res.PageSize,
            TotalItems = res.TotalItems
        };
    }
}
