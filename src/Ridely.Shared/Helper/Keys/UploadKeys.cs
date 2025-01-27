namespace Soloride.Shared.Helper.Keys;
public static class UploadKeys
{
    public static class Driver
    {
        public static string DriversLicense(long driverId) =>
            $"DriversLicense/Driver-DriversLicense-{driverId}";

        public static string ProfileImage(long driverId) =>
            $"DriversProfileImage/Driver-ProfileImage-{driverId}";
    }

    public static class Rider
    {
        public static string ProfileImage(long riderId) =>
            $"RidersProfileImage/Rider-ProfileImage-{riderId}";
    }
}
