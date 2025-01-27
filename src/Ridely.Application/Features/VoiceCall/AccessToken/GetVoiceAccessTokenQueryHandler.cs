using Soloride.Application.Abstractions.Messaging;
using Soloride.Application.Abstractions.VoiceCall;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.VoiceCall.AccessToken;
internal sealed class GetVoiceAccessTokenQueryHandler :
    IQueryHandler<GetVoiceAccessTokenQuery, GetVoiceAccessTokenResponse>
{
    private readonly IRiderRepository _riderRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IVoiceService _voiceService;

    public GetVoiceAccessTokenQueryHandler(IRiderRepository riderRepository,
        IDriverRepository driverRepository, IRideRepository rideRepository,
        IVoiceService voiceService)
    {
        _riderRepository = riderRepository;
        _driverRepository = driverRepository;
        _rideRepository = rideRepository;
        _voiceService = voiceService;
    }

    public async Task<Result<GetVoiceAccessTokenResponse>> Handle(GetVoiceAccessTokenQuery request,
        CancellationToken cancellationToken)
    {
        var ride = await _rideRepository
            .GetAsync(request.RideId);

        if (ride is null) return Error.NotFound("ride.notfound", "Ride not found");

        if (!ride.DriverId.HasValue)
            return Error.BadRequest("ride.nomatch", "Ride has not been matched yet");

        if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null)
                return Error.NotFound("rider.notfound", "Rider not found");

            if (string.IsNullOrEmpty(rider.PhoneNo))
                return Error.BadRequest("no.phoneno", "Rider has no phone number");

            (string accessToken, string channel) = await _voiceService
                .GenerateAgoraAccessTokenAsync(ride.Id.ToString(), false);

            return new GetVoiceAccessTokenResponse(accessToken, channel);
        }

        if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository
                .GetAsync(request.DriverId.Value);

            if (driver is null)
                return Error.NotFound("driver.notfound", "Driver not found");

            if (string.IsNullOrWhiteSpace(driver.PhoneNo))
                return Error.BadRequest("no.phoneno", "Driver has no phone number");

            (string accessToken, string channel) = await _voiceService
                .GenerateAgoraAccessTokenAsync(ride.Id.ToString(), true);

            return new GetVoiceAccessTokenResponse(accessToken, channel);
        }

        return new GetVoiceAccessTokenResponse("", "");
    }
}
