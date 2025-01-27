using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Rides;
using Soloride.Domain.Rides;

namespace Soloride.Infrastructure.Repositories;
internal sealed class ChatRepository(ApplicationDbContext context) :
    GenericRepository<Chat>(context), IChatRepository
{
    public async Task<PaginatedList<ChatModel>> SearchAsync(ChatSearchQueryParams searchParams)
    {
        var query = context.Set<Chat>()
            .Include(x => x.Ride.Rider)
            .Include(x => x.Ride.Driver)
            .AsQueryable();

        query = query.Where(x => x.RideId == searchParams.RideId);

        if (searchParams.RiderId.HasValue)
            query = query.Where(x => x.SenderId == searchParams.RiderId.Value || x.RecipientId == searchParams.RiderId.Value);

        if (searchParams.DriverId.HasValue)
            query = query.Where(x => x.SenderId == searchParams.DriverId.Value || x.RecipientId == searchParams.DriverId.Value);

        var projections = query.Select(x => new ChatModel
        {
            Id = x.Id,
            Sender = x.Sender.ToString(),
            SenderName = x.Sender.ToString() == nameof(ChatUserType.Rider) ? x.Ride.Rider.FirstName + " " + x.Ride.Rider.LastName :
                    x.Ride.Driver.FirstName + " " + x.Ride.Driver.LastName,
            Recipient = x.Recipient.ToString(),
            RecipientName = x.Recipient.ToString() == nameof(ChatUserType.Rider) ? x.Ride.Rider.FirstName + " " + x.Ride.Rider.LastName :
                    x.Ride.Driver.FirstName + " " + x.Ride.Driver.LastName,
            Message = x.Message,
            CreatedAt = x.CreatedAtUtc
        }).OrderByDescending(x => x.CreatedAt);

        int totalRecords = await query.CountAsync();

        int skip = (searchParams.PageNumber - 1) * searchParams.PageSize;

        var records = await projections
            .Skip(skip)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return new PaginatedList<ChatModel>
        {
            Items = records,
            PageSize = searchParams.PageSize,
            PageNumber = searchParams.PageNumber,
            TotalItems = totalRecords
        };
    }
}
