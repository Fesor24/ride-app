using Soloride.Domain.Call;

namespace Soloride.Infrastructure.Repositories
{
    internal sealed class CallLogRepository(ApplicationDbContext context) :
        GenericRepository<CallLog>(context), ICallLogRepository
    {
    }
}
