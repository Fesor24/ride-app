using Soloride.Contracts.Models;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;

namespace Soloride.Application.Abstractions.Rides;
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
