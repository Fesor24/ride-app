using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Storage;
using Ridely.Application.Abstractions.VoiceCall;
using Ridely.Contracts.Events;
using RidelyAPI.Controllers.Base;
using StackExchange.Redis;

namespace RidelyAPI.Controllers;

public class TestController(ISmsService smsService, IDeviceNotificationService deviceNotificationService, 
    IConnectionMultiplexer connectionMultiplexer, 
    IVoiceService voiceService, IObjectStoreService objectStoreService, IPaystackService paystackService,
    IPublishEndpoint publishEndpoint) : 
    BaseController<TestController>
{
    private readonly StackExchange.Redis.IDatabase _db = connectionMultiplexer.GetDatabase();

    [HttpGet("api/sms")]
    public async Task<IActionResult> SendSms(string phoneNo)
    {
        await smsService.SendVerificationCodeAsync("08186088250", "89212", "5");

        return Ok("message sent");
    }

    [HttpGet("api/push-notification/{deviceTokenId}")]
    public async Task<IActionResult> TestPushNotifications(string deviceTokenId)
    {
        var res = await deviceNotificationService.PushAsync(deviceTokenId, "Backend", "Wale is the best");

        return Ok(res);
    }

    [HttpGet("api/hello")]
    public async Task<IActionResult> Hello()
    {
        await publishEndpoint.Publish(new RideRequestedEvent
        {
            AvailableDriverProfile = []
        });


        return Ok("Hello world!!");
    }

    [HttpGet("api/redis")]
    public async Task<IActionResult> RedisTest()
    {
        double lat = 6.60;
        double longi = 6.60;

        await _db.GeoAddAsync("Test-Location", longi, lat, "1");

        var result = await _db.GeoSearchAsync("Test-Location",
        longi, lat, new GeoSearchCircle(4000, GeoUnit.Meters),
            order: Order.Ascending, options: GeoRadiusOptions.WithDistance);

        return Ok(result);
    }

    [HttpGet("api/agora/test-token")]
    public async Task<IActionResult> AgoraToken()
    {
        var res = await voiceService.GenerateAgoraAccessTokenAsync("3", true);

        return Ok(res.Token);
    }

    //[HttpPost("api/upload/images")]
    //public async Task<IActionResult> Images(ImageUploadTest image)
    //{
    //    string profileKey = UploadKeys.Driver.ProfileImage(4);

    //    string licenseKey = UploadKeys.Driver.DriversLicense(4);

    //    await objectStoreService.UploadAsync(profileKey, image.Profile);

    //    await objectStoreService.UploadAsync(licenseKey, image.DriversLicense);

    //    var profile = await objectStoreService.GeneratePreSignedUrl(profileKey);
    //    var license = await objectStoreService.GeneratePreSignedUrl(licenseKey);

    //    return Ok(new
    //    {
    //        profile = profile.Url,
    //        license = license.Url
    //    });
    //}

    

}

public class ImageUploadTest
{
    public string Profile { get; set; }
    public string DriversLicense { get; set; }
}

public class TestTermii
{
    [JsonPropertyName("api_key")]
    public string ApiKey { get; set; } = "TLNUyxGmICQZHVQGBdVcXpRWRSDtUkckxzNzJhvLxjxianBHoAnCahqeCRlCZi";

    [JsonPropertyName("to")]
    public string To { get; set; } = "2349033016038";

    [JsonPropertyName("from")]
    public string From { get; set; } = "N-Alert";

    [JsonPropertyName("channel")]
    public string Channel { get; set; } = "dnd";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "plain";

    [JsonPropertyName("sms")]
    public string Sms { get; set; } = "Your (Ridely) Verification Pin is 2002. Valid for 5 minutes, one-time use only.(Ridely)";
}
