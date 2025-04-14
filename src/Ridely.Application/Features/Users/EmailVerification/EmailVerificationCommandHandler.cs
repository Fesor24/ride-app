using Microsoft.AspNetCore.SignalR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Application.Features.Users.EmailVerification;
internal sealed class EmailVerificationCommandHandler : ICommandHandler<EmailVerificationCommand>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<RideHub> _rideHubContext;

    public EmailVerificationCommandHandler(IDriverRepository driverRepository, IRiderRepository riderRepository,
        IUnitOfWork unitOfWork, IHubContext<RideHub> rideHubContext)
    {
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _unitOfWork = unitOfWork;
        _rideHubContext = rideHubContext;
    }

    public async Task<Result<bool>> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
    {
        if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetAsync(request.DriverId.Value);

            if (driver is null) return new NotFound("driver.notfound", "Driver not found");

            //if (driver.EmailValidated) return true;

            driver.ValidateEmail();

            _driverRepository.Update(driver);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _rideHubContext.Clients.User(DriverKey.CustomNameIdentifier(driver.Id))
            .SendAsync(SignalRSubscription.EmailVerificationUpdate, new
            {
                Message = "Email verified",
                Verified = true
            }, cancellationToken);

            return true;
        }

        else if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository.GetAsync(request.RiderId.Value);

            if (rider is null) return new NotFound("rider.notfound", "Rider not found");

            //if (rider.EmailValidated) return true;

            rider.ValidateEmail();

            _riderRepository.Update(rider);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _rideHubContext.Clients.User(RiderKey.CustomNameIdentifier(rider.Id))
                .SendAsync(SignalRSubscription.EmailVerificationUpdate, new
                {
                    Message = "Email verified",
                    Verified = true
                }, cancellationToken);

            return true;
        }

        return false;
    }
}
