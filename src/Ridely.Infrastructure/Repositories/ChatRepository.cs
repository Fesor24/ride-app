using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;

namespace Ridely.Infrastructure.Repositories;
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
            query = query.Where(x => x.Sender == UserType.Rider || x.Recipient == UserType.Rider);

        if (searchParams.DriverId.HasValue)
            query = query.Where(x => x.Sender == UserType.Driver || x.Recipient == UserType.Driver);

        var projections = query.Select(x => new ChatModel
        {
            Id = x.Id,
            Sender = x.Sender.ToString(),
            SenderName = x.Sender.ToString() == nameof(UserType.Rider) ? x.Ride.Rider.FirstName + " " + x.Ride.Rider.LastName :
                    x.Ride.Driver.FirstName + " " + x.Ride.Driver.LastName,
            Recipient = x.Recipient.ToString(),
            RecipientName = x.Recipient.ToString() == nameof(UserType.Rider) ? x.Ride.Rider.FirstName + " " + x.Ride.Rider.LastName :
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
