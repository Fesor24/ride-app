using Ridely.Domain.Common;

namespace Ridely.Infrastructure.Repositories;
internal sealed class SettingsRepository(ApplicationDbContext context) :
    GenericRepository<Settings>(context), ISettingsRepository
{
}
