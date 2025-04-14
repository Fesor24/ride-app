using Amazon.S3.Model.Internal.MarshallTransformations;
using MediatR;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Features.Users.EmailVerification;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;

namespace Ridely.Application.Features.Users.InitiateEmailVerification;
internal sealed class InitiateEmailVerificationCommandHandler : ICommandHandler<InitiateEmailVerificationCommand>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly ISender _sender;

    public InitiateEmailVerificationCommandHandler(IDriverRepository driverRepository,
        IRiderRepository riderRepository, ISender sender)
    {
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _sender = sender;
    }

    public async Task<Result<bool>> Handle(InitiateEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetAsync(request.DriverId.Value);

            if (driver is null) return new NotFound("driver.notfound", "Driver not found");

            //if (driver.EmailValidated) return true;

            // send email...

            // for now
            // temp...call verify email handler...
            var command = new EmailVerificationCommand(request.DriverId.Value, null);

            await _sender.Send(command);

            return true;
        }

        else if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository.GetAsync(request.RiderId.Value);

            if (rider is null) return new NotFound("rider.notfound", "Rider not found");

            //if (rider.EmailValidated) return true;

            // send email...

            // temp...call verify email handler...
            var command = new EmailVerificationCommand(null, request.RiderId.Value);

            await _sender.Send(command);

            return true;
        }

        return false;
    }
}
