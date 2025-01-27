using Microsoft.EntityFrameworkCore;
using Soloride.Application.Helper;
using Soloride.Domain.Models;
using Soloride.Domain.Models.Riders;
using Soloride.Domain.Riders;

namespace Soloride.Infrastructure.Repositories;
internal sealed class RiderTransactionHistoryRepository(ApplicationDbContext context) :
    GenericRepository<RiderTransactionHistory>(context), IRiderTransactionHistoryRepository
{
    private readonly ApplicationDbContext _context = context;

    public Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference) =>
        _context.Set<RiderTransactionHistory>()
        .FirstOrDefaultAsync(transaction => transaction.Reference == reference);

    public async Task<RiderTransactionHistory?> GetByReferenceAsync(Ulid reference, RiderTransactionType transactionType)
    {
        return await _context.Set<RiderTransactionHistory>()
            .FirstOrDefaultAsync(trx => trx.Reference == reference && trx.Type == transactionType);
    }

    public async Task<PaginatedList<RiderTransactionModel>> Search(RiderTransactionSearchParams searchParams)
    {
        var query = _context.Set<RiderTransactionHistory>()
            .Include(x => x.Rider)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchParams.PhoneNo))
            query = query.Where(ride => ride.Rider.PhoneNo == DataFormatter.FormatPhoneNo(searchParams.PhoneNo));

        if (!string.IsNullOrWhiteSpace(searchParams.Reference))
            query = query.Where(ride => ride.Reference.ToString() == searchParams.Reference);

        if (searchParams.From.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date >= searchParams.From.Value);

        if (searchParams.To.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date <= searchParams.To.Value);

        if (searchParams.Status.HasValue)
            query = query.Where(x => x.Status == searchParams.Status.Value);

        if (searchParams.Types.Any())
            query = query.Where(x => searchParams.Types.Contains(x.Type));

        int totalItems = await query.CountAsync();

        var queryProjection = query.Select(x => new RiderTransactionModel
        {
            Reference = x.Reference.ToString(),
            Amount = x.Amount,
            Status = x.Status,
            Rider = x.Rider.FirstName + " " + x.Rider.LastName,
            CreatedAt = x.CreatedAtUtc
        }).OrderByDescending(x => x.CreatedAt);

        int take = searchParams.PageSize;

        int skip = (searchParams.PageNumber - 1) * take;

        var records = await queryProjection
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedList<RiderTransactionModel>
        {
            Items = records,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = totalItems
        };
    }
}
