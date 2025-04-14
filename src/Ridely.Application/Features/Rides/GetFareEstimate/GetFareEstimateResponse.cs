using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.GetFareEstimate;
public sealed class GetFareEstimateResponse
{
    public long RideId { get; set; }
    public DateTime EstimatedTimeofArrival { get; set; }
    public double DurationInSeconds { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public decimal DiscountInPercent { get; set; }
    public List<RideCategoryFareEstimate> RideCategoryEstimates { get; set; }
}

public class RideCategoryFareEstimate
{
    public int DriversCount { get; set; }
    public RideCategory RideCategory { get; set; }
    public long EstimatedFare { get; set; }
    public long DiscountedEstimatedFare { get; set; }
    public int PassengerCapacity { get; set; }
}
