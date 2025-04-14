using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Models.Riders;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Riders.Get;
internal sealed class GetRiderQueryHandler :
    IQueryHandler<GetRiderQuery, GetRiderResponse>
{
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderReferrersRepository _riderReferrersRepository;

    public GetRiderQueryHandler(IRiderRepository riderRepository, IRiderReferrersRepository riderReferrersRepository)
    {
        _riderRepository = riderRepository;
        _riderReferrersRepository = riderReferrersRepository;
    }

    // todo: see if rider owes money for pending ride...mobile devs show pop up so he can proceed to make payment....
    public async Task<Result<GetRiderResponse>> Handle(GetRiderQuery request, CancellationToken cancellationToken)
    {
        RiderModel? rider = await _riderRepository
            .GetDetailsAsync(request.RiderId);

        if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

        var riderResponse = new GetRiderResponse()
        {
            AvailableBalance = rider.AvailableBalance,
            DeviceTokenId = rider.DeviceTokenId,
            Email = rider.Email,
            PhoneNo = rider.PhoneNo,
            FirstName = rider.FirstName,
            LastName = rider.LastName,
            ProfileImage = rider.ProfileImageUrl,
            Status = rider.Status,
            EmailVerified = rider.EmailVerified,
        };

        var referredUsers = await _riderReferrersRepository
            .GetReferredUsersCount(rider.Id);

        riderResponse.ReferralInfo.RidersReferred = referredUsers.Riders;
        riderResponse.ReferralInfo.DriversReferred = referredUsers.Drivers;

        if (!string.IsNullOrWhiteSpace(rider.ReferralCode))
            riderResponse.ReferralInfo.ReferralCode = rider.ReferralCode.ToUpperInvariant();

        return riderResponse;
    }
}
