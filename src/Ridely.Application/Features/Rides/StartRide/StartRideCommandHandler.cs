using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Rides;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.StartRide;
internal sealed class StartRideCommandHandler :
    ICommandHandler<StartRideCommand, StartRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideRepository _rideRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IPaymentService _paymentService;
    private readonly IHubContext<RideHub> _rideHubContext;

    public StartRideCommandHandler(IUnitOfWork unitOfWork,
        IRideRepository rideRepository, IRideLogRepository rideLogRepository, IDriverRepository driverRepository,
        IPaymentService paymentService, IHubContext<RideHub> rideHubContext)
    {
        _unitOfWork = unitOfWork;
        _rideRepository = rideRepository;
        _rideLogRepository = rideLogRepository;
        _driverRepository = driverRepository;
        _paymentService = paymentService;
        _rideHubContext = rideHubContext;
    }

    public async Task<Result<StartRideResponse>> Handle(StartRideCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetRideDetails(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Status == RideStatus.Started) 
            return Error.BadRequest("ride.started", "This ride has been started already");

        if (ride.Status != RideStatus.Arrived)
            return Error.BadRequest("ride.drivernotarrived", "Driver not at pickup");

        ride.Driver.UpdateStatus(DriverStatus.InTrip, ride.Id);

        ride.UpdateStatus(RideStatus.Started);

        RideLog rideLog = new(ride.Id, RideLogEvent.Started);

        await _rideLogRepository.AddAsync(rideLog);

        _rideRepository.Update(ride);

        _driverRepository.Update(ride.Driver);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var source = ride.GetCoordinates(ride.SourceCordinates);

        var destination = ride.GetCoordinates(ride.DestinationCordinates);

        await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(ride.RiderId))
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                Update = ReceiveRideUpdate.Started,
                Data = JsonSerializer.Serialize(new
                {
                    Message = "Ride started"
                })
            });

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(ride.RiderId.ToString());

        //var startRideMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDE_START,
        //    new
        //    {
        //        message = "Ride started"
        //    });

        //await _webSocketManager.SendMessageAsync(riderWebSocketKey, startRideMessage);

        BackgroundJob.Schedule(() => HandleCardTripPayment(ride.Id,
            cancellationToken), DateTime.UtcNow.AddSeconds(40));

        string[] waypointAddressess = ride.WaypointAddresses.Split("%%");

        var waypointsCoordinates = ride.GetWaypointCoordinates();

        List<StartRideResponse.RideLocation> waypoints = [];

        for(int i = 0; i < waypointsCoordinates.Count; i++)
        {
            StartRideResponse.RideLocation location = new()
            {
                Address = waypointAddressess[i],
                Latitude = waypointsCoordinates[i].Latitude,
                Longitude = waypointsCoordinates[i].Longitude
            };

            waypoints.Add(location);
        }

        return new StartRideResponse
        {
            Destination = new StartRideResponse.RideLocation
            {
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                Address = ride.DestinationAddress
            },
            Source = new StartRideResponse.RideLocation
            {
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                Address = ride.SourceAddress
            },
            Waypoints = waypoints,
            PaymentMethod = ride.Payment.Method,
            RideConversation = ride.HaveConversation,
            MusicGenre = ride.MusicGenre
        };
    }

    public async Task<Result> HandleCardTripPayment(long rideId, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetRideDetails(rideId);

        // it will never be null...
        if (ride is null) return Result.Success();

        if (ride.Payment.Method != PaymentMethod.Card) return Result.Success();

        var result = await _paymentService
            .ProcessCardTripPaymentAsync(ride, ride.EstimatedFare, cancellationToken);

        if (result.IsSuccessful)
        {
            int maximumDurationInSeconds = 7 * 60;// 7 mins

            if(ride.EstimatedDurationInSeconds <= maximumDurationInSeconds)
                maximumDurationInSeconds = ride.EstimatedDurationInSeconds - (3 * 60);// reduce by 2 mins...

            BackgroundJob.Schedule(() => _paymentService.VerifyCardPaymentAndUpdatePaymentMethodToCashIfFailAsync(ride),
                DateTime.UtcNow.AddSeconds(maximumDurationInSeconds));

            return Result.Success();
        }

        await _paymentService.UpdatePaymentMethodToCashAndNotifyUsersAsync(ride);

        return Result.Success();
    }
}
