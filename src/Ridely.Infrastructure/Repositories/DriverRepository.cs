using Microsoft.EntityFrameworkCore;
using Ridely.Application.Helper;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class DriverRepository(ApplicationDbContext context) :
    GenericRepository<Driver>(context), IDriverRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Driver?> GetDetailsAsync(long driverId)
    {
        return await _context.Set<Driver>()
            .Include(x => x.Cab)
            .Where(x => x.Id == driverId)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetPhoneNoAsync(long driverId) =>
        await _context.Set<Driver>()
            .Where(x => x.Id == driverId)
            .Select(x => x.PhoneNo)
            .FirstOrDefaultAsync();
    public async Task<Driver?> GetByPhoneNoAsync(string phoneNo) =>
        await _context.Set<Driver>()
        .Include(driver => driver.Cab)
        .FirstOrDefaultAsync(x => x.PhoneNo == DataFormatter.FormatPhoneNo(phoneNo));

    public async Task<Driver?> GetByReferralCodeAsync(string referralCode) =>
        await _context.Set<Driver>()
        .FirstOrDefaultAsync(x => x.ReferralCode == referralCode.Trim().ToLower());

    public async Task<PaginatedList<DriverModel>> Search(DriverSearchParams searchParams)
    {
        var query = _context.Set<Driver>()
            .Include(x => x.Cab)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchParams.Email))
            query = query.Where(x => x.Email == searchParams.Email.ToLower().Trim());

        if (!string.IsNullOrWhiteSpace(searchParams.PhoneNo))
            query = query.Where(x => x.PhoneNo == searchParams.PhoneNo.Trim());

        var projections = query.Select(x => new DriverModel
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email ?? "",
            PhoneNo = x.PhoneNo ?? "",
            CreatedAt = x.CreatedAtUtc,
            Cab = new Domain.Models.Common.CabModel
            {
                Type = x.Cab.CabType.ToString()
            },
            AvgRatings = x.AvgRatings,
            CompletedRides = x.CompletedTrips
        })
            .OrderByDescending(x => x.CreatedAt);

        var totalRecords = await projections.CountAsync();

        int skip = (searchParams.PageNumber - 1) * searchParams.PageSize;

        var records = await projections
            .Skip(skip)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return new PaginatedList<DriverModel>
        {
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            Items = records,
            TotalItems = totalRecords
        };
    }

    public async Task<Driver?> GetByEmailAsync(string email) =>
         await _context.Set<Driver>()
            .FirstOrDefaultAsync(driver => driver.Email == email);

    public async Task<int> GetTotalCountAsync() =>
        await _context.Set<Driver>()
        .CountAsync();
}
