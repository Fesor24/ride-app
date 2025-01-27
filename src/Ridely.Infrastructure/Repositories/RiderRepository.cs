using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Soloride.Application.Helper;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Riders;
using Soloride.Domain.Riders;

namespace Soloride.Infrastructure.Repositories;
internal sealed class RiderRepository(ApplicationDbContext context) :
    GenericRepository<Rider>(context), IRiderRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<RiderModel?> GetDetailsAsync(long riderId) =>
        await _context.Set<Rider>()
            .Include(x => x.PaymentCards)
            .Where(x => x.Id == riderId)
            .Select(x => new RiderModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Status = x.Status,
                Email = x.Email,
                PhoneNo = x.PhoneNo,
                ReferralCode = x.ReferralCode,
                DeviceTokenId = x.DeviceTokenId,
                ProfileImageUrl = x.ProfileImageUrl,
                Cards = x.PaymentCards
                .Select(y => new CardModel
                {
                    Id = y.Id,
                    CardType = y.CardType.ToString(),
                    Last4Digits = y.Last4Digits,
                    ExpiryMonth = y.ExpiryMonth,
                    ExpiryYear = y.ExpiryYear
                }).ToList()
            })
        .FirstOrDefaultAsync();

    public async Task<Rider?> GetByPhoneNoAsync(string phoneNo) =>
        await _context.Set<Rider>()
        .FirstOrDefaultAsync(x => x.PhoneNo == DataFormatter.FormatPhoneNo(phoneNo));

    public async Task<string?> GetPhoneNoAsync(long riderId) =>
        await _context.Set<Rider>()
            .Where(x => x.Id == riderId)
            .Select(x => x.PhoneNo)
            .FirstOrDefaultAsync();

    public async Task<PaginatedList<RiderModel>> Search(RiderSearchParams searchParams)
    {
        var query = _context.Set<Rider>()
            .AsQueryable();

        if(searchParams.From.HasValue)
            query = query.Where(rider => rider.CreatedAtUtc >= searchParams.From.Value);

        if(searchParams.To.HasValue)
            query = query.Where(rider => rider.CreatedAtUtc <= searchParams.To.Value);

        if (!string.IsNullOrWhiteSpace(searchParams.Email))
            query = query.Where(rider => rider.Email.Contains(searchParams.Email));

        if (!string.IsNullOrWhiteSpace(searchParams.PhoneNo))
            query = query.Where(rider => rider.PhoneNo == searchParams.PhoneNo);

        var projections = query.Select(x => new RiderModel
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            PhoneNo = x.PhoneNo,
            CreatedAt = x.CreatedAtUtc
        })
            .OrderByDescending(x => x.CreatedAt);

        var totalRecords = await projections.CountAsync();

        int skip = (searchParams.PageNumber - 1) * searchParams.PageSize;

        var records = await projections
            .Skip(skip)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return new PaginatedList<RiderModel>
        {
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            Items = records,
            TotalItems = totalRecords
        };
    }

    public async Task<Rider?> GetByEmailAsync(string email)
    {
        return await _context.Set<Rider>()
            .FirstOrDefaultAsync(rider => rider.Email == email);
    }

    public async Task<Rider?> GetByReferralCodeAsync(string referralCode)
    {
        return await _context.Set<Rider>()
            .FirstOrDefaultAsync(rider => rider.ReferralCode == referralCode.ToLower());
    }

    public async Task<int> GetTotalCountAsync() =>
        await _context.Set<Rider>().CountAsync();
}
