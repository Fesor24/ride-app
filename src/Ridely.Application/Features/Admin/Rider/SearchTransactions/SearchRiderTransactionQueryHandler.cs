using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Models;
using Ridely.Domain.Models.Riders;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Admin.Rider.SearchTransactions;
internal sealed class SearchRiderTransactionQueryHandler : 
    IQueryHandler<SearchRiderTransactionQuery, PaginatedList<SearchRiderTransactionResponse>>
{
    private readonly IRiderTransactionHistoryRepository _riderTransactionHistoryRepository;

    public SearchRiderTransactionQueryHandler(IRiderTransactionHistoryRepository riderTransactionHistoryRepository)
    {
        _riderTransactionHistoryRepository = riderTransactionHistoryRepository;
    }

    public async Task<Result<PaginatedList<SearchRiderTransactionResponse>>> Handle(SearchRiderTransactionQuery request, 
        CancellationToken cancellationToken)
    {
        var searchParams = new RiderTransactionSearchParams
        {
            From = request.From,
            To = request.To,
            Status = request.Status,
            Types = request.Types,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            PhoneNo = request.PhoneNo,
            Reference = request.Reference
        };

        var result = await _riderTransactionHistoryRepository.Search(searchParams);

        return new PaginatedList<SearchRiderTransactionResponse>
        {
            Items = result.Items.ConvertAll(trx => new SearchRiderTransactionResponse
            {
                Amount = trx.Amount,
                Status = trx.Status.ToString(),
                CreatedAt = trx.CreatedAt.ToCustomDateString(),
                Reference = trx.Reference,
                Rider = trx.Rider
            }),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems
        };
    }
}
