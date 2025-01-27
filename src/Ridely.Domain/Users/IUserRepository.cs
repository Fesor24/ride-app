using Ridely.Domain.Abstractions;
using Ridely.Domain.Models.Users;

namespace Ridely.Domain.Users;
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneNoAsync(string phoneNo);
}
