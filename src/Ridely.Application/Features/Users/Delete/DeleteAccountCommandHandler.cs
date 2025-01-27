using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Users.Delete;
internal sealed class DeleteAccountCommandHandler :
    ICommandHandler<DeleteAccountCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDriverRepository _driverRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IPaymentCardRepository _paymentCardRepository;
    private readonly IBankAccountRepository _bankAccountRepository;

    public DeleteAccountCommandHandler(IUnitOfWork unitOfWork, IDriverRepository driverRepository,
        IRiderRepository riderRepository, IPaymentCardRepository paymentCardRepository,
        IBankAccountRepository bankAccountRepository)
    {
        _unitOfWork = unitOfWork;
        _driverRepository = driverRepository;
        _riderRepository = riderRepository;
        _paymentCardRepository = paymentCardRepository;
        _bankAccountRepository = bankAccountRepository;
    }

    public async Task<Result<bool>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        // todo: Remove images from cloud after 20 days...
        if (request.RiderId.HasValue)
        {
            var rider = await _riderRepository
                .GetAsync(request.RiderId.Value);

            if (rider is null) return Error.NotFound("rider.notfound", "Rider not found");

            if (rider.Status == RiderStatus.InTrip)
                return Error.BadRequest("ride.inprogress", "Account can not be deleted while ride in progress");

            rider.Delete();

            _riderRepository.Update(rider);

            var paymentCards = await _paymentCardRepository
                .GetAllByRiderAsync(rider.Id);

            if (paymentCards.Count > 0)
            {
                _paymentCardRepository.DeleteRange(paymentCards);
            }

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
        else if (request.DriverId.HasValue)
        {
            var driver = await _driverRepository
                .GetDetailsAsync(request.DriverId.Value);

            if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

            if (driver.Status == DriverStatus.InTrip)
                return Error.BadRequest("ride.inprogress", "Account can not be deleted while ride in progress");

            driver.Delete();

            _driverRepository.Update(driver);

            var driverBankAccounts = await _bankAccountRepository
                .GetAllByDriverAsync(driver.Id);

            if (driverBankAccounts.Count > 0)
            {
                _bankAccountRepository.DeleteRange(driverBankAccounts);
            }

            return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
        else
            return false;
    }
}
