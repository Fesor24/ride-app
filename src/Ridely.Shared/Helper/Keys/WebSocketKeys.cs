namespace Soloride.Shared.Helper.Keys;
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
