namespace Soloride.Application.Abstractions.Rides;
public sealed record EstimatedFareResponse(
    ulong EstimatedFare,
    ulong DeliveryFare,
    int DurationInSeconds,
    int DistanceInMeters);
