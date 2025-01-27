using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.VoiceCall.AccessToken;
public sealed record GetVoiceAccessTokenQuery(long RideId, long? RiderId, long? DriverId) :
    IQuery<GetVoiceAccessTokenResponse>;
