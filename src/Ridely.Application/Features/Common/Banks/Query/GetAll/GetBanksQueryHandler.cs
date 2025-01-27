using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Application.Features.Common.Banks.Query.GetAll;
internal sealed class GetBanksQueryHandler(IBankRepository bankRepository) :
    IQueryHandler<GetBanksQuery, List<GetBankResponse>>
{
    public async Task<Result<List<GetBankResponse>>> Handle(GetBanksQuery request, 
        CancellationToken cancellationToken)
    {
        var banks = await bankRepository.GetAllBanks();

        return banks.ConvertAll(x => new GetBankResponse
        {
            Id = x.Id,
            Name = x.Name
        });
    }
}
