using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Application.Features.Drivers.Transactions;
internal sealed class SearchDriverTransactionsQueryHandler : IQueryHandler<SearchDriverTransactionsQuery,
    PaginatedList<SearchDriverTransactionsResponse>>
{
    private readonly IDriverTransactionHistoryRepository _driverTransactionHistoryRepository;

    public SearchDriverTransactionsQueryHandler(IDriverTransactionHistoryRepository driverTransactionHistoryRepository)
    {
        _driverTransactionHistoryRepository = driverTransactionHistoryRepository;
    }

    public async Task<Result<PaginatedList<SearchDriverTransactionsResponse>>> Handle(SearchDriverTransactionsQuery request, 
        CancellationToken cancellationToken)
    {
        var searchParams = new DriverTransactionSearchParams
        {
            DriverId = request.DriverId,
            Types = [],
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        var result = await _driverTransactionHistoryRepository.Search(searchParams);

        return new PaginatedList<SearchDriverTransactionsResponse>
        {
            Items = result.Items.ConvertAll(trx => new SearchDriverTransactionsResponse
            {
                Amount = trx.Amount,
                CreatedAt = trx.CreatedAt.ToCustomDateString(),
                Status = trx.Status,
                Type = trx.Type
            }),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
