namespace Ridely.Application.Abstractions.Rides;
public sealed record EstimatedFareResponse(
    long EstimatedFare,
    long DeliveryFare,
    int DurationInSeconds,
    int DistanceInMeters);
