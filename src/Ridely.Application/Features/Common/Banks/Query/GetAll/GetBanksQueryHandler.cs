using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;

namespace Soloride.Application.Features.Common.Banks.Query.GetAll;
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
