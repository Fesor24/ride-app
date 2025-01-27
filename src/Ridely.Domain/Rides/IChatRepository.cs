using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Rides;

namespace Ridely.Domain.Rides;
public interface IChatRepository : IGenericRepository<Chat>
{
    Task<PaginatedList<ChatModel>> SearchAsync(ChatSearchQueryParams searchParams);
}
