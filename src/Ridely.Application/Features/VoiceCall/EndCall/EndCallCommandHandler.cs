using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Call;

namespace Ridely.Application.Features.VoiceCall.EndCall
{
    internal sealed class EndCallCommandHandler(IUnitOfWork unitOfWork, ICallLogRepository callLogRepository) : 
        ICommandHandler<EndCallCommand>
    {
        public async Task<Result<bool>> Handle(EndCallCommand request, CancellationToken cancellationToken)
        {
            var callLog = await callLogRepository.GetAsync(request.CallId);

            if (callLog is null) return Error.NotFound("calllog.notfound", "Call log not found");

            if (callLog.CallEndUtc.HasValue) return true;

            callLog.EndCall();

            callLogRepository.Update(callLog);

            return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
