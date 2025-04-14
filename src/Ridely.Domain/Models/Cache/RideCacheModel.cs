namespace Ridely.Domain.Models.Cache;
public sealed record RideCacheModel(long RideId, long RiderId, 
    long DriverId, string DriverDeviceTokenId, string RiderDeviceTokenId);
