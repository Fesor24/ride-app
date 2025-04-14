using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.RequestWaitTime;
internal sealed class RequestWaitTimeCommandHandler : ICommandHandler<RequestWaitTimeCommand, RequestWaitTimeResponse>
{
    private readonly IWaitTimeRepository _waitTimeRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPushNotificationService _pushNotificationService;

    public RequestWaitTimeCommandHandler(IWaitTimeRepository waitTimeRepository, IRideRepository rideRepository,
        IHubContext<RideHub> rideHubContext, IDriverRepository driverRepository, IRideLogRepository rideLogRepository,
        IUnitOfWork unitOfWork, IPushNotificationService pushNotificationService)
    {
        _waitTimeRepository = waitTimeRepository;
        _rideRepository = rideRepository;
        _rideHubContext = rideHubContext;
        _driverRepository = driverRepository;
        _rideLogRepository = rideLogRepository;
        _unitOfWork = unitOfWork;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<Result<RequestWaitTimeResponse>> Handle(RequestWaitTimeCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetAsync(request.RideId);

        if (ride is null) 
            return Error.NotFound("ride.notfound", "Ride not found");

        List<RideStatus> acceptableStatuses = [RideStatus.Matched, RideStatus.Arrived];

        if (!acceptableStatuses.Contains(ride.Status))
            return Error.BadRequest("invalid.ridestatus",
                "Extension can only be requested when there's a ride match or driver arrived at pickup");

        var waitTime = await _waitTimeRepository.GetAsync(request.WaitTimeId);

        if (waitTime is null)
            return Error.NotFound("waittime.notfound", "Wait time option not found");

        long rideLogId = 0;

        if (ride.DriverId.HasValue)
        {
            await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(ride.DriverId.Value))
             .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
             {
                 Update = ReceiveRideUpdate.RequestWaitTimeExtension,
                 Data = JsonSerializer.Serialize(new
                 {
                     Message = $"Wait time requested for {waitTime.Minute} minutes",
                     Minutes = waitTime.Minute,
                     waitTime.Amount,
                     WaitTimeId = waitTime.Id
                 })
             }, cancellationToken);

            var driver = await _driverRepository.GetAsync(ride.DriverId.Value);

            if (driver is not null && !string.IsNullOrWhiteSpace(driver.DeviceTokenId))
            {
                Dictionary<string, string> data = new()
                {
                    {"minutes", waitTime.Minute.ToString() },
                    {"amount", waitTime.Amount.ToString() },
                    {"waitTimeId", waitTime.Id.ToString() },
                };

                await _pushNotificationService.PushAsync(
                    driver.DeviceTokenId,
                    "Ride cancelled",
                    "Ride has been cancelled",
                    data,
                    PushNotificationType.RequestWaitTimeExtension);
            }

            WaitTimeRequestedEventDetails eventDetails = new(waitTime.Id, waitTime.Amount, waitTime.Minute, default);

            RideLog rideLog = new RideLog(ride.Id, RideLogEvent.WaitTimeRequested);

            rideLog.SetDetails(eventDetails);

            await _rideLogRepository.AddAsync(rideLog);

            await _unitOfWork.SaveChangesAsync();

            rideLogId = rideLog.Id;
        }

        return new RequestWaitTimeResponse(rideLogId);
    }
}
