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
using Ridely.Infrastructure.WebSockets;
using Ridely.Infrastructure.WebSockets.Handlers;
using Ridely.Api.Controllers.Base;
using StackExchange.Redis;

namespace Ridely.Api.Controllers;

public class TestController(ISmsService smsService, IDeviceNotificationService deviceNotificationService, 
    IConnectionMultiplexer connectionMultiplexer, 
    IVoiceService voiceService, IObjectStoreService objectStoreService, IPaystackService paystackService,
    IPublishEndpoint publishEndpoint, WebSocketEventHandler webSocketEventHandler) : 
    BaseController<TestController>
{
    private readonly StackExchange.Redis.IDatabase _db = connectionMultiplexer.GetDatabase();
    private readonly WebSocketEventHandler _webSocketEventHandler = webSocketEventHandler;

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
        var wsEvent = new WebSocketEvent("TEST.HELLO")
        {
            EventArgs =
            {
                {"name", "Fesor"}
            }
        };

        await _webSocketEventHandler.DispatchAsync(wsEvent);

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
