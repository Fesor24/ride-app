using Ridely.Domain.Call;

namespace Ridely.Infrastructure.Repositories
{
    internal sealed class CallLogRepository(ApplicationDbContext context) :
        GenericRepository<CallLog>(context), ICallLogRepository
    {
    }
}
