using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models.Users;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Users;

namespace Ridely.Infrastructure.Repositories;
internal sealed class UserRepository(ApplicationDbContext context) :
    GenericRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await context.Set<User>()
        .FirstOrDefaultAsync(x => x.Email == email.ToLower().Trim());

    public async Task<User?> GetByPhoneNoAsync(string phoneNo) =>
        await context.Set<User>()
        .FirstOrDefaultAsync(x => x.PhoneNo == phoneNo.Trim());

    public override async Task<User?> GetAsync(long userId) =>
        await context.Set<User>()
        .Include(x => x.Role)
        .FirstOrDefaultAsync(x => x.Id == userId);
}
