﻿namespace Ridely.Shared.Constants;
public static class ApplicationConstant
{
    public static int DRIVER_RESPONSETIME_INSECONDS = 90;
    public static int RIDER_UPDATEPAYMENT_RESPONSETIME_INSECONDS = 30;
    public static string REDIS_LOCATIONKEY = "LOCATION";
    public static int CALL_WAIT_TIME = 20;
    //public static string RedisLocationKey(string area) => $"LOCATION-{area.Replace(" ", "").ToUpperInvariant()}";
}
