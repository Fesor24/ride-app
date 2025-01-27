using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Call;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.VoiceCall.StartCall
{
    internal sealed class StartCallCommandHandler : 
        ICommandHandler<StartCallCommand, StartCallResponse>
    {
        private readonly IRideRepository _rideRepository;
        private readonly ICallLogRepository _callLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartCallCommandHandler(IRideRepository rideRepository,
            ICallLogRepository callLogRepository, IUnitOfWork unitOfWork)
        {
            _rideRepository = rideRepository;
            _callLogRepository = callLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StartCallResponse>> Handle(StartCallCommand request, CancellationToken cancellationToken)
        {
            var ride = await _rideRepository
                .GetAsync(request.RideId);

            if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

            CallLog callLog = new(
                ride.Id,
                request.DriverCalled ? CallLogUser.Rider : CallLogUser.Driver,
                request.DriverCalled ? CallLogUser.Driver : CallLogUser.Rider
                );

            await _callLogRepository.AddAsync(callLog);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StartCallResponse(callLog.Id);
        }
    }
}
