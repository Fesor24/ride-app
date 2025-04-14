using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.VerifyCardPayment;
internal sealed class VerifyCardPaymentCommandHandler : ICommandHandler<VerifyCardPaymentCommand>
{
    private readonly IPaymentService _paymentService;
    private readonly IRideRepository _rideRepository;

    public VerifyCardPaymentCommandHandler(IPaymentService paymentService, IRideRepository rideRepository)
    {
        _paymentService = paymentService;
        _rideRepository = rideRepository;
    }

    public async Task<Result<bool>> Handle(VerifyCardPaymentCommand request, CancellationToken cancellationToken)
    {
        var ride = await _rideRepository.GetRideDetails(request.RideId);

        if (ride is null)
            return Error.NotFound("ride.notfound", "Ride not found");

        List<RideStatus> rideStatuses = [RideStatus.Started, RideStatus.Completed];

        if (!rideStatuses.Contains(ride.Status))
            return Error.BadRequest("invalid.ridestatus", "Payment can be verified when ride has started");

        await _paymentService.VerifyCardPaymentAndUpdatePaymentMethodToCashIfFailAsync(ride);

        return true;
    }
}
