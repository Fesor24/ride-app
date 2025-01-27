using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.VoiceCall.AccessToken;
public sealed record GetVoiceAccessTokenQuery(long RideId, long? RiderId, long? DriverId) :
    IQuery<GetVoiceAccessTokenResponse>;
