using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Riders.Events;
using Soloride.Shared.Helper;

namespace Soloride.Application.Features.Riders.RegisterRider;
internal sealed class RiderRegisteredDomainEventHandler : INotificationHandler<RiderRegisteredDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderReferrersRepository _riderReferrersRepository;
    private readonly IDriverReferrersRepository _driverReferrersRepository;
    private readonly IDriverRepository _driverRepository;

    public RiderRegisteredDomainEventHandler(IUnitOfWork unitOfWork, IRiderRepository riderRepository,
        IRiderReferrersRepository riderReferrersRepository, IDriverReferrersRepository driverReferrersRepository,
        IDriverRepository driverRepository)
    {
        _unitOfWork = unitOfWork;
        _riderRepository = riderRepository;
        _riderReferrersRepository = riderReferrersRepository;
        _driverReferrersRepository = driverReferrersRepository;
        _driverRepository = driverRepository;
    }

    public async Task Handle(RiderRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var rider = await _riderRepository
            .GetByPhoneNoAsync(notification.PhoneNo);

        if (rider is null) return;

        await GenerateReferrerCodeAndHandleReferrer(notification.ReferredByCode, rider);

        _riderRepository.Update(rider);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task GenerateReferrerCodeAndHandleReferrer(string referredByCode, Domain.Riders.Rider rider)
    {
        string firstName = rider.FirstName;

        if (firstName.Length < 3)
            firstName = firstName.PadRight(3, '0');
        else
            firstName = firstName[..3];

        string referralCode = firstName + "-sr" + RandomGenerator.GenerateReferralCode(rider.Id);

        rider.UpdateReferralCode(referralCode);

        if (string.IsNullOrWhiteSpace(referredByCode)) return;

        string[] referredBySplit = referredByCode.Split("-");

        if (referredBySplit.Length != 2) return;

        string referCodeQualifier = referredBySplit[1];

        if (referCodeQualifier.StartsWith("sd", StringComparison.InvariantCultureIgnoreCase))
        {
            // driver was the person that referred this user
            var referredByDriver = await _driverRepository
                .GetByReferralCodeAsync(referredByCode);

            if (referredByDriver is null) return;

            DriverReferrers driverReferrers = new(referredByDriver.Id,
                rider.Id, Domain.Shared.ReferredUser.Rider);

            await _driverReferrersRepository.AddAsync(driverReferrers);

            rider.UpdateReferredBy(referredByDriver.Id, Domain.Shared.ReferredUser.Driver);
        }

        else if (referCodeQualifier.StartsWith("sr", StringComparison.InvariantCultureIgnoreCase))
        {
            // rider was the person that referred this user...
            var referredByRider = await _riderRepository
                .GetByReferralCodeAsync(referredByCode);

            if (referredByRider is null) return;

            RiderReferrers riderReferrers = new(referredByRider.Id,
                rider.Id, Domain.Shared.ReferredUser.Rider);

            await _riderReferrersRepository.AddAsync(riderReferrers);

            rider.UpdateReferredBy(referredByRider.Id, Domain.Shared.ReferredUser.Rider);
        }
    }
}
