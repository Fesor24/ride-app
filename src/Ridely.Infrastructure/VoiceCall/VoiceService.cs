using AgoraIO.Media;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.VoiceCall;
using Ridely.Infrastructure.VoiceCall;

namespace Ridely.Infrastructure.Services;
internal class VoiceService : IVoiceService
{
    private readonly AgoraCredentials _agoraCredentials;

    public VoiceService(IOptions<AgoraCredentials> agoraCredentials)
    {
        _agoraCredentials = agoraCredentials.Value;
    }

    //public async Task<string> GenerateTwilioAccessTokenAsync(string clientIdentity)
    //{
    //    // Get from app settings
    //    string accountSid = _twilioSettings.AccountSid; // the account sid
    //    string apiKeySid = _twilioSettings.ApiKeySid; // the api key sid
    //    string apiKeySecret = _twilioSettings.ApiKeySecret; // the api key secret
    //    string twilioApplicationSid = _twilioSettings.ApplicationSid; // Create a twilio ml app this...optional param??

    //    // 306d8f42071a46e99ba35bcc390491a1..app id for agora...306d8f42071a46e99ba35bcc390491a1
    //    // 3e9399bef1c747e49ff76d0b6e70093a...app certificate

    //    // push credential sid...should this depend on the device....
    //    // for the service...ISe336a50e2159d8a561ff979aa1173652
    //    // for fcm specifically...CRcb0151ff5c6451f362813c6dd7eae92d
    //    var grant = new VoiceGrant()
    //    {
    //        IncomingAllow = true,
    //        OutgoingApplicationSid = twilioApplicationSid,
    //        PushCredentialSid = "CRcb0151ff5c6451f362813c6dd7eae92d"
    //    };

    //    var token = new Token(
    //        accountSid: accountSid,
    //        apiKeySid, apiKeySecret, identity: clientIdentity,
    //        grants: new HashSet<IGrant> { grant}, expiration: DateTime.UtcNow.AddMinutes(60)
    //        );

    //    return await Task.FromResult(token.ToJwt());
    //}

    public async Task<(string Token, string Channel)> GenerateAgoraAccessTokenAsync(string rideId, bool isDriver)
    {
        //string appId = "306d8f42071a46e99ba35bcc390491a1";
        string appId = _agoraCredentials.AppId;
        //string appCertificate = "3e9399bef1c747e49ff76d0b6e70093a";
        string appCertificate = _agoraCredentials.AppCertificate;
        string channelName = $"room-{rideId}";
        uint uid = 1;
        uint tokenExpire = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // Token valid for 1 hour
        uint privilegeExpire = (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600; // Privilege valid for 1 hour

        if(isDriver)
            uid = 2;

        string token = RtcTokenBuilder.buildTokenWithUID(appId, appCertificate, channelName, uid,
                        RtcTokenBuilder.Role.RolePublisher, privilegeExpire);


        return await Task.FromResult((token, channelName));
    }
}
