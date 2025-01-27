using Ridely.Contracts.Models;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;

namespace Ridely.Application.Abstractions.Rides;
public interface IRideService
{
    Task<bool> SendRequestToDriversAsync(Domain.Models.Location ridersLocation,
        RideObject ride, Rider rider,
        List<long> excludeDrivers, RideCategory rideCategory, 
        bool increaseSearchRadius = false);
    Task<Result<EstimatedFareResponse>> ComputeEstimatedFare(Domain.Models.Location source,
        Domain.Models.Location destination);
    Task SendChatMessageAsync(ChatUserType sender, string message, string identifier, long riderId);
}
