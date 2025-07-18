﻿using Ridely.Application.Abstractions.Location;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Rides;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Exceptions;

namespace Ridely.Application.Features.Rides.GetFareEstimate;
internal sealed class GetFareEstimateCommandHandler :
    ICommandHandler<GetFareEstimateCommand, GetFareEstimateResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRideService _rideService;
    private readonly ILocationService _locationService;
    private readonly IRideRepository _rideRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly PricingService _pricingService;

    public GetFareEstimateCommandHandler(IUnitOfWork unitOfWork, IRideService rideService,
        ILocationService locationService, IRideRepository rideRepository, IRiderRepository riderRepository,
        ISettingsRepository settingsRepository, IPaymentRepository paymentRepository, PricingService pricingService)
    {
        _unitOfWork = unitOfWork;
        _rideService = rideService;
        _locationService = locationService;
        _rideRepository = rideRepository;
        _riderRepository = riderRepository;
        _settingsRepository = settingsRepository;
        _paymentRepository = paymentRepository;
        _pricingService = pricingService;
    }

    public async Task<Result<GetFareEstimateResponse>> Handle(GetFareEstimateCommand request,
        CancellationToken cancellationToken)
    {
        Domain.Riders.Rider? rider = await _riderRepository.GetAsync(request.RiderId);

        if (rider is null)
            return Error.NotFound("rider.notfound", "Rider not found");

        Domain.Models.Location? waypoint = null;

        if(request.Waypoint is not null)
        {
            waypoint = new()
            {
                Latitude = request.Waypoint.Latitude,
                Longitude = request.Waypoint.Longitude
            };
        }

        var res = await _rideService.ComputeEstimatedFare(new Domain.Models.Location
        {
            Latitude = request.Source.Latitude,
            Longitude = request.Source.Longitude
        }, new Domain.Models.Location
        {
            Latitude = request.Destination.Latitude,
            Longitude = request.Destination.Longitude
        }, waypoint);

        if (res.IsFailure) return res.Error;

        string origin = request.SourceAddress, destination = request.DestinationAddress;

        decimal discountInPercent = await GetUserAvailableDiscount();

        long estimatedFare = _pricingService.ConvertPriceInDecimalToLong(res.Value.EstimatedFare);

        long estimatedFareDiscounted = _pricingService.ConvertPriceInDecimalToLong(res.Value.EstimatedFare, discountInPercent);

        long estimatedDeliveryFare = _pricingService.ConvertPriceInDecimalToLong(res.Value.DeliveryFare);

        // default payment to cash
        Payment ridePayment = new(
            PaymentMethod.Cash);

        await _paymentRepository.AddAsync(ridePayment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        Ride ride = Ride.CreateRide(
            rider.Id,
            estimatedFare,
            estimatedDeliveryFare,
            request.Source.Latitude,
            request.Source.Longitude,
            request.Destination.Latitude,
            request.Destination.Longitude,
            ridePayment.Id,
            res.Value.DistanceInMeters,
            request.SourceAddress,
            request.DestinationAddress,
            res.Value.DurationInSeconds
            );

        if(request.Waypoint is not null)
        {
            ride.UpdateWaypointCoordinates(request.Waypoint.Latitude, request.Waypoint.Longitude);
            ride.UpdateWaypointAddresses(request.WayPointAddress ?? "");
        }

        await _rideRepository.AddAsync(ride);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var settings = await _settingsRepository
            .GetAllAsync();

        if (settings.Count == 0)
            throw new ApiNotFoundException("Settings not found");

        int premiumCabPrice = settings.First().PremiumCab;

        int economyCabs = await _locationService.GetAvailableDriversCountInLocationAsync(
            new Domain.Models.Location
            {
                Latitude = request.Source.Latitude,
                Longitude = request.Source.Longitude
            },
            [], rider.Id, CabType.Economy, DriverService.Car);

        int premiumCabs = await _locationService.GetAvailableDriversCountInLocationAsync(
            new Domain.Models.Location
            {
                Latitude = request.Source.Latitude,
                Longitude = request.Source.Longitude
            },
             [], rider.Id, CabType.Premium, DriverService.Car);

        int deliveryBikes = await _locationService.GetAvailableDriversCountInLocationAsync(
            new Domain.Models.Location
            {
                Latitude = request.Source.Latitude,
                Longitude = request.Source.Longitude
            },
             [], rider.Id, null, DriverService.Delivery);

        return new GetFareEstimateResponse
        {
            RideId = ride.Id,
            EstimatedTimeofArrival = DateTime.UtcNow.AddSeconds(res.Value.DurationInSeconds),
            Source = origin,
            Destination = destination,
            DurationInSeconds = res.Value.DurationInSeconds,
            DiscountInPercent = discountInPercent,
            RideCategoryEstimates =
            [
                new RideCategoryFareEstimate
                {
                    RideCategory  = RideCategory.CabEconomy,
                    DriversCount = economyCabs,
                    EstimatedFare = estimatedFare,
                    DiscountedEstimatedFare = estimatedFareDiscounted,
                    PassengerCapacity = 4
                },
                new RideCategoryFareEstimate
                {
                    RideCategory = RideCategory.CabPremium,
                    DriversCount = premiumCabs,
                    EstimatedFare = premiumCabPrice + estimatedFare,
                    DiscountedEstimatedFare = estimatedFareDiscounted,
                    PassengerCapacity = 4
                },
                new RideCategoryFareEstimate
                {
                    RideCategory = RideCategory.Delivery,
                    DriversCount = deliveryBikes,
                    EstimatedFare = estimatedDeliveryFare,
                    PassengerCapacity = 0
                }
            ]
        };
    }

    private Task<decimal> GetUserAvailableDiscount()
    {
        return Task.FromResult(5.0m);
    }
}
