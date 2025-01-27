using Soloride.Domain.Common;

namespace Soloride.Infrastructure.Repositories;
internal sealed class SettingsRepository(ApplicationDbContext context) :
    GenericRepository<Settings>(context), ISettingsRepository
{
}
