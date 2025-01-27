namespace Soloride.Application.Abstractions.VoiceCall;
public interface IVoiceService
{
    Task<(string Token, string Channel)> GenerateAgoraAccessTokenAsync(string rideId, bool isDriver);
}
