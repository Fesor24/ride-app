using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Rides.Reroute;
internal sealed class RerouteRideCommandHandler :
    ICommandHandler<RerouteRideCommand, RerouteRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRideLogRepository _rideLogRepository;
    private readonly IRideService _rideService;
    private readonly IWebSocketManager _webSocketManager;

    public RerouteRideCommandHandler(IUnitOfWork unitOfWork, IRideRepository rideRepository,
        IRiderRepository riderRepository, IRideLogRepository rideLogRepository, IRideService rideService,
        IWebSocketManager webSocketManager)
    {
        _unitOfWork = unitOfWork;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _rideLogRepository = rideLogRepository;
        _rideService = rideService;
        _webSocketManager = webSocketManager;
    }

    public async Task<Result<RerouteRideResponse>> Handle(RerouteRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.Status != RideStatus.InTransit)
            return Error.BadRequest("ride.notinprogress", "Only rides in progress can be extended");

        ride.UpdateWaypointCoordinates(request.Source.Latitude, request.Source.Longitude);

        ride.UpdateWaypointAddresses(request.SourceAddress);

        ride.UpdateDestinationAddress(request.DestinationAddress);

        ride.UpdateDestinationCordinates(request.Destination.Latitude, request.Destination.Longitude);

        // todo: recalculate based on the distance covered...do not factor in the time here
        // use the initial estimated fare and the distance covered...
        // reason is traffic might differ between requests...
        var getCompletedDistanceFare = await _rideService.ComputeEstimatedFare(ride.GetCoordinates(ride.SourceCordinates),
            request.Source);

        if (getCompletedDistanceFare.IsFailure) return getCompletedDistanceFare.Error;

        var completedDistanceFare = getCompletedDistanceFare.Value.EstimatedFare;

        var res = await _rideService.ComputeEstimatedFare(request.Source, request.Destination);

        if (res.IsFailure) return res.Error;

        //todo: update the new fare
        //ride.EstimatedFare = res.Value.EstimatedFare + completedDistanceFare;

        _rideRepository.Update(ride);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string driverWebSocketKey = WebSocketKeys.Driver.Key(ride.DriverId!.Value.ToString());
        var driverRideExtensionRequestMessage = new WebSocketResponse<object>
        {
            Event = SocketEventConstants.RIDE_DESTINATIONUPDATE,
            Payload = new
            {
                source = new
                {
                    address = request.SourceAddress,
                    latitude = request.Source.Latitude,
                    longitude = request.Source.Longitude,
                },
                destination = new
                {
                    address = request.DestinationAddress,
                    latitude = request.Destination.Latitude,
                    longitude = request.Destination.Longitude
                }
            }

        };

        string requestMessage = Serialize.Object(driverRideExtensionRequestMessage);

        await _webSocketManager.SendMessageAsync(driverWebSocketKey, requestMessage);

        return new RerouteRideResponse
        {
            DurationInSeconds = res.Value.DurationInSeconds,
            EstimatedTimeOfArrival = DateTime.UtcNow.AddSeconds(res.Value.DurationInSeconds),
            DestinationCoordinates = request.Destination,
            DestinationAddress = request.DestinationAddress
        };
    }
}
