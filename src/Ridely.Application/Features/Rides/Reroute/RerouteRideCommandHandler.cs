using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Rides;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Rides.Reroute;
internal sealed class RerouteRideCommandHandler :
    ICommandHandler<RerouteRideCommand, RerouteRideResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideRepository _rideRepository;
    private readonly IRideService _rideService;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly IPaymentService _paymentService;

    public RerouteRideCommandHandler(IUnitOfWork unitOfWork, IRideRepository rideRepository,
         IRideService rideService, IHubContext<RideHub> rideHubContext, IPaymentService paymentService)
    {
        _unitOfWork = unitOfWork;
        _rideRepository = rideRepository;
        _rideService = rideService;
        _rideHubContext = rideHubContext;
        _paymentService = paymentService;
    }

    public async Task<Result<RerouteRideResponse>> Handle(RerouteRideCommand request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        if (ride.WasRerouted)
            return Error.BadRequest("ride.rerouted", "A ride can only be rerouted once");

        // for now...reroute can be done once...
        if (ride.Status != RideStatus.Started)
            return Error.BadRequest("ride.notinprogress", "Only rides in progress can be extended");

        //ride.UpdateWaypointCoordinates(request.Source.Latitude, request.Source.Longitude, true);

        //ride.UpdateWaypointAddresses(request.SourceAddress);

        //ride.UpdateDestinationAddress(request.DestinationAddress);

        //ride.UpdateDestinationCordinates(request.Destination.Latitude, request.Destination.Longitude);

        ride.Reroute(request.DestinationAddress, request.Destination.Latitude, request.Destination.Longitude);

        _rideRepository.Update(ride);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var getCompletedDistanceFare = await _rideService.ComputeEstimatedFare(ride.GetCoordinates(ride.SourceCordinates),
            request.Source);

        if (getCompletedDistanceFare.IsFailure) return getCompletedDistanceFare.Error;

        var completedDistanceFare = getCompletedDistanceFare.Value.EstimatedFare;

        var res = await _rideService.ComputeEstimatedFare(request.Source, request.Destination);

        if (res.IsFailure) return res.Error;

        BackgroundJob.Enqueue(() => _paymentService
            .ProcessReroutedTripPaymentAsync(ride, completedDistanceFare, res.Value.EstimatedFare));

        //string driverWebSocketKey = WebSocketKeys.Driver.Key(ride.DriverId!.Value.ToString());

        //var rerouteRideMessage = WebSocketMessage<object>.Create(
        //    SocketEventConstants.RIDE_DESTINATIONUPDATE,
        //    new
        //    {
        //        source = new
        //        {
        //            address = request.SourceAddress,
        //            latitude = request.Source.Latitude,
        //            longitude = request.Source.Longitude,
        //        },
        //        destination = new
        //        {
        //            address = request.DestinationAddress,
        //            latitude = request.Destination.Latitude,
        //            longitude = request.Destination.Longitude
        //        }
        //    });

        await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(ride.DriverId!.Value))
            .SendAsync(SignalRSubscription.ReceiveRideUpdates, new
            {
                Update = ReceiveRideUpdate.Rerouted,
                Data = JsonSerializer.Serialize(new
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
                })
            });

        //await _webSocketManager.SendMessageAsync(driverWebSocketKey, rerouteRideMessage);

        return new RerouteRideResponse
        {
            DurationInSeconds = res.Value.DurationInSeconds,
            EstimatedTimeOfArrival = DateTime.UtcNow.AddSeconds(res.Value.DurationInSeconds),
            DestinationCoordinates = request.Destination,
            DestinationAddress = request.DestinationAddress
        };
    }
}
