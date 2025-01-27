using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Drivers;

namespace Ridely.Application.Features.Admin.Driver.SearchTransactions;
internal sealed class SearchDriverTransactionQueryHandler : 
    IQueryHandler<SearchDriverTransactionQuery, PaginatedList<SearchDriverTransactionResponse>>
{
    private readonly IDriverTransactionHistoryRepository _driverTransactionHistoryRepository;

    public SearchDriverTransactionQueryHandler(IDriverTransactionHistoryRepository driverTransactionHistoryRepository)
    {
        _driverTransactionHistoryRepository = driverTransactionHistoryRepository;
    }

    public async Task<Result<PaginatedList<SearchDriverTransactionResponse>>> Handle(SearchDriverTransactionQuery request, 
        CancellationToken cancellationToken)
    {
        var searchParams = new DriverTransactionSearchParams
        {
            Types = request.Types,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Status = request.Status,
            PhoneNo = request.PhoneNo,
            From = request.From,
            Reference = request.Reference,
            To = request.To,
        };

        var result = await _driverTransactionHistoryRepository.Search(searchParams);

        return new PaginatedList<SearchDriverTransactionResponse>
        {
            Items = result.Items.ConvertAll(trx => new SearchDriverTransactionResponse
            {
                Amount = trx.Amount,
                Reference = trx.Reference,
                CreatedAt = trx.CreatedAt.ToCustomDateString(),
                Driver = trx.Driver,
                Status = trx.Status.ToString()
            }),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
