using Hangfire;
using MediatR;
using Ridely.Application.Abstractions.Authentication;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Extensions;
using Ridely.Application.Features.Rides.EndRide;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Riders;
using Ridely.Domain.Riders.Events;
using RiderDomain = Ridely.Domain.Riders.Rider;

namespace Ridely.Application.Features.Riders.RegisterRider;
internal sealed class RegisterRiderCommandHandler :
    ICommandHandler<RegisterRiderCommand, RegisterRiderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _tokenService;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderWalletRepository _riderWalletRepository;
    private readonly IPublisher _publisher;

    public RegisterRiderCommandHandler(IUnitOfWork unitOfWork, IJwtService tokenService,
        IRiderRepository riderRepository, IRiderWalletRepository riderWalletRepository, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _riderRepository = riderRepository;
        _riderWalletRepository = riderWalletRepository;
        _publisher = publisher;
    }

    public async Task<Result<RegisterRiderResponse>> Handle(RegisterRiderCommand request, CancellationToken cancellationToken)
    {
        string phoneNo = request.PhoneNo.ToPhoneNumber();

        bool riderWithPhoneNoExist = await _riderRepository
            .GetByPhoneNoAsync(phoneNo) != null;

        if (riderWithPhoneNoExist) return Error.BadRequest("phoneno.exist", "User with phone no exist");

        string email = request.Email
            .Replace(" ", "")
            .Trim()
            .ToLowerInvariant();

        bool riderWithEmailExist = await _riderRepository
            .GetByEmailAsync(email) != null;

        if (riderWithEmailExist) return Error.BadRequest("email.exist", "User with email exist");

        RiderDomain rider = RiderDomain.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            phoneNo,
            request.Gender,
            request.ReferrerCode ?? "",
            null
            );

        await _riderRepository.AddAsync(rider);

        //rider.RaiseDomainEvent(new RiderRegisteredDomainEvent(phoneNo, request.ReferrerCode ?? ""));



        await _unitOfWork.SaveChangesAsync(cancellationToken);

        RiderWallet wallet = new(rider.Id);

        await _riderWalletRepository.AddAsync(wallet);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        (string accessToken, string refreshToken) = await _tokenService.GenerateToken(rider);

        string referrerCode = request.ReferrerCode ?? "";

        BackgroundJob.Enqueue(() => _publisher.Publish(new RiderRegisteredDomainEvent(phoneNo, referrerCode), cancellationToken));

        return new RegisterRiderResponse(accessToken, refreshToken);
    }
}
