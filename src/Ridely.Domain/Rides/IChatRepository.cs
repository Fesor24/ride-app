using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Rides;

namespace Soloride.Domain.Rides;
public interface IChatRepository : IGenericRepository<Chat>
{
    Task<PaginatedList<ChatModel>> SearchAsync(ChatSearchQueryParams searchParams);
}
