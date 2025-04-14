using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.GetWaitTime;
internal sealed class GetWaitTimeQueryHandler :
    IQueryHandler<GetWaitTimeQuery, IReadOnlyList<GetWaitTimeResponse>>
{
    private readonly IWaitTimeRepository _waitingTimeRepository;

    public GetWaitTimeQueryHandler(IWaitTimeRepository waitingTimeRepository)
    {
        _waitingTimeRepository = waitingTimeRepository;
    }

    public async Task<Result<IReadOnlyList<GetWaitTimeResponse>>> Handle(GetWaitTimeQuery request,
        CancellationToken cancellationToken)
    {
        var waitingTimes = await _waitingTimeRepository.GetAllAsync();

        return waitingTimes.ConvertAll(wt => new GetWaitTimeResponse(wt.Id, wt.Amount, wt.Minute));
    }
}
