using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Extensions;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Rides;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Rides.GetChatMessages;
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
