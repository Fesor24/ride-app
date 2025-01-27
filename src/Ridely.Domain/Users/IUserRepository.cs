using Soloride.Domain.Abstractions;
using Soloride.Domain.Models.Users;

namespace Soloride.Domain.Users;
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNoAsync(string phoneNo);
}
