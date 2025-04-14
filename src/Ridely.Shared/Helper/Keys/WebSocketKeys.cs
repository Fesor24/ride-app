using System.Net.NetworkInformation;

namespace Ridely.Shared.Helper.Keys;
public static class WebSocketKeys
{
    public static class Rider
    {
        public static string Key(string riderId) => $"RIDERWS-{riderId}";
    }

    public static class Driver
    {
        public static string Key(string driverId) => $"DRIVERWS-{driverId}";
    }
}

public abstract class BaseKey
{
    public virtual long GetIdenfierFromClaim(string identifier)
    {
        string[] split = identifier.Split("-");

        if (split.Length != 2) return 0;

        if (long.TryParse(split[1], out long id)) return id;

        return 0;
    }
}

public class DriverKey : BaseKey
{
    public static string LocationUpdates(long driverId) => $"DRIVERLOC-{driverId}";
    public static string LocationSubscribers(long driverId) => $"DRIVERLOCSUB-{driverId}";
    public static string CustomNameIdentifier(long driverId) => $"DRIVERID-{driverId}";
    public override long GetIdenfierFromClaim(string identifier)
    {
        return base.GetIdenfierFromClaim(identifier) ;
    }
}

public class RiderKey : BaseKey
{
    public static string CustomNameIdentifier(long riderId) => $"RIDERID-{riderId}";
    public static string Disconnected(long riderId) => $"RIDERDISC-{riderId}";
    public override long GetIdenfierFromClaim(string identifier)
    {
        return base.GetIdenfierFromClaim(identifier);
    }
}
