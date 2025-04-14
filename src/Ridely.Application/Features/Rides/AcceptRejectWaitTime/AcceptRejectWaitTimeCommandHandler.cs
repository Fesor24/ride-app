using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Features.Rides.CancelRideRequest;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.AcceptRejectWaitTime;
internal sealed class AcceptRejectWaitTimeCommandHandler : ICommandHandler<AcceptRejectWaitTimeCommand>
{
    private readonly IRideRepository _rideRepository;
    private readonly ISender _sender;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly IWaitTimeRepository _waitTimeRepository;
    private readonly IPaymentDetailRepository _paymentDetailRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AcceptRejectWaitTimeCommandHandler(IRideRepository rideRepository, ISender sender,
        IHubContext<RideHub> rideHubContext, IWaitTimeRepository waitTimeRepository,
        IPaymentDetailRepository paymentDetailRepository, IRideLogRepository rideLogRepository, IUnitOfWork unitOfWork)
    {
        _rideRepository = rideRepository;
        _sender = sender;
        _rideHubContext = rideHubContext;
        _waitTimeRepository = waitTimeRepository;
        _paymentDetailRepository = paymentDetailRepository;
        _rideLogRepository = rideLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AcceptRejectWaitTimeCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        if (request.AcceptWaitTimeExtension)
        {
            var rideLog = await _rideLogRepository.GetAsync(request.RideLogId);

            if (rideLog is null)
                return Error.NotFound("ridelog.notfound", "Ride log nor found");

            if (rideLog.Event != RideLogEvent.WaitTimeRequested)
                return Error.BadRequest("invalid.ridelogevent", "Invalid ride log event");

            if (string.IsNullOrWhiteSpace(rideLog.Details))
                return Error.BadRequest("logdetails.notfound", "Unable to accept this extension");

            var eventDetails = JsonSerializer.Deserialize<WaitTimeRequestedEventDetails>(rideLog.Details);

            if(eventDetails is null)
                return Error.BadRequest("eventdetail.deserializationerror", "Unable to accept this extension");

            var waitTime = await _waitTimeRepository.GetAsync(eventDetails.WaitTimeId);

            if (waitTime is null)
                return Error.NotFound("waittime.notfound", "Wait time not found");

            rideLog.SetDetails(new WaitTimeRequestedEventDetails(eventDetails.WaitTimeId,
                eventDetails.Amount, eventDetails.TimeInMinutes, DateTime.UtcNow));

            _rideLogRepository.Update(rideLog);

            PaymentDetail? paymentDetail = await _paymentDetailRepository.GetAsync(PaymentFor.WaitTime, ride.PaymentId);

            if(paymentDetail is null)
            {
                PaymentDetail newPaymentDetail = new(Ulid.NewUlid(), ride.PaymentId, PaymentFor.WaitTime, eventDetails.Amount);

                await _paymentDetailRepository.AddAsync(newPaymentDetail);
            }
            else
            {
                paymentDetail.IncreaseOrOverrideAmountDue(eventDetails.Amount);

                _paymentDetailRepository.Update(paymentDetail);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(ride.RiderId))
             .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
             {
                 Update = ReceiveRideUpdate.WaitTimeExtensionStatus,
                 Data = JsonSerializer.Serialize(new
                 {
                     Message = $"Wait time extended by {eventDetails.TimeInMinutes} minutes"
                 })
             }, cancellationToken);

            return true;
        }

        else
        {
            var command = new CancelRideRequestCommand(request.RideId, request.Reason ?? "", false, UserType.Driver);

            return await _sender.Send(command, cancellationToken);
        }

    }
}
