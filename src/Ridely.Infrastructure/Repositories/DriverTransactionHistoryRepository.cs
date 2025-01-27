using Microsoft.EntityFrameworkCore;
using Ridely.Application.Helper;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Infrastructure.Repositories;
internal sealed class DriverTransactionHistoryRepository(ApplicationDbContext context) :
    GenericRepository<DriverTransactionHistory>(context), IDriverTransactionHistoryRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<DriverTransactionHistory?> GetByReferenceAsync(Ulid reference) =>
        await _context.Set<DriverTransactionHistory>()
            .FirstOrDefaultAsync(transaction => transaction.Reference == reference);

    public async Task<PaginatedList<DriverTransactionModel>> Search(DriverTransactionSearchParams searchParams)
    {
        var query = _context.Set<DriverTransactionHistory>()
            .Include(x => x.Driver)
            .AsQueryable();

        if(searchParams.DriverId.HasValue)
            query = query.Where(trx => trx.DriverId == searchParams.DriverId.Value);

        if (!string.IsNullOrWhiteSpace(searchParams.PhoneNo))
            query = query.Where(trx => trx.Driver.PhoneNo == DataFormatter.FormatPhoneNo(searchParams.PhoneNo));

        if (!string.IsNullOrWhiteSpace(searchParams.Reference))
            query = query.Where(trx => trx.Reference.ToString() == searchParams.Reference);

        if (searchParams.From.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date >= searchParams.From.Value);

        if (searchParams.To.HasValue)
            query = query.Where(x => x.CreatedAtUtc.Date <= searchParams.To.Value);

        if (searchParams.Status.HasValue)
            query = query.Where(x => x.Status == searchParams.Status.Value);

        if (searchParams.Types.Any())
            query = query.Where(x => searchParams.Types.Contains(x.Type));

        int totalItems = await query.CountAsync();

        var queryProjection = query.Select(x => new DriverTransactionModel
        {
            Reference = x.Reference.ToString(),
            Amount = x.Amount,
            Status = x.Status,
            Driver = x.Driver.FirstName + " " + x.Driver.LastName,
            Type = x.Type,
            CreatedAt = x.CreatedAtUtc
        }).OrderByDescending(x => x.CreatedAt);

        int take = searchParams.PageSize;

        int skip = (searchParams.PageNumber - 1) * take;

        var records = await queryProjection
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return new PaginatedList<DriverTransactionModel>
        {
            Items = records,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = totalItems
        };
    }
}
