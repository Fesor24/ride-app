using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Payments;
using Ridely.Domain.Transactions;

namespace Ridely.Infrastructure.Repositories;
internal sealed class TransactionHistoryRepository(ApplicationDbContext context)
{
   

    //public async Task<PaginatedList<TransactionModel>> Search(TransactionsSearchParams searchParams)
    //{
    //    var query = context.Set<TransactionHistory>()
    //        .AsQueryable();

    //    if (searchParams.Type.HasValue)
    //        query = query.Where(x => x.Type == searchParams.Type.Value);

    //    int totalItems = await query.CountAsync();

    //    var queryProjection = query.Select(x => new TransactionModel
    //    {
    //        Id = x.Id,
    //        Amount = x.Amount,
    //        Reference = x.Reference,
    //        Status = x.Status,
    //        CreatedAt = x.CreatedAtUtc,
    //        Type = x.Type
    //    }).OrderByDescending(x => x.CreatedAt);

    //    int take = searchParams.PageSize;

    //    int skip = (searchParams.PageNumber - 1) * take;

    //    var records = await queryProjection
    //        .Skip(skip)
    //        .Take(take)
    //        .ToListAsync();

    //    return new PaginatedList<TransactionModel>
    //    {
    //        Items = records,
    //        PageNumber = searchParams.PageNumber,
    //        PageSize = searchParams.PageSize,
    //        TotalItems = totalItems
    //    };
    //}
}
