namespace Ridely.Shared.Helper.Keys;
public static class RideKeys
{
    public static string RiderRequestRide(long riderId) => $"RIDER.RIDEREQUEST-{riderId}";
    public static string RideNotMatched(long rideId) => $"RIDE.NOTMATCHED-{rideId}";

    // value is the rider id so location updates can be forwarded...
    public static string Matched(string driverId) => $"DRIVER.MATCHED-{driverId}";

    public static string DriverLocation(string driverId) => $"DRIVER.LOCATION-{driverId}";

    public static string RideRequestToDriver(string driverId) => $"DRIVER.RIDEREQUEST-{driverId}";

    public static string RideCancelled(string rideId) => $"RIDE.REQUESTCANCELLED-{rideId}";

    public static string Ride(string rideId) => $"RIDEOBJ-{rideId}";

    public static string DriversCancelled(string riderId) => $"RIDE.DRIVERSCANCELLED-{riderId}";
}
