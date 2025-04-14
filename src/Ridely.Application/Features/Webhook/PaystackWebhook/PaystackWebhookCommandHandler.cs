using System.Text.Json;
using Hangfire;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Transactions;

namespace Ridely.Application.Features.Webhook.PaystackWebhook;
internal sealed class PaystackWebhookCommandHandler :
    ICommandHandler<PaystackWebhookCommand>
{
    private readonly ITransactionLogRepository _transactionLogRepository;
    private readonly IRiderRepository _riderRepository;
    private readonly IRiderTransactionHistoryRepository _riderTransactionHistoryRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverTransactionHistoryRepository _driverTransactionHistoryRepository;
    private readonly IRiderWalletRepository _riderWalletRepository;
    private readonly IDriverWalletRepository _driverWalletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITransactionReferenceMapRepository _transactionReferenceMapRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentCardRepository _paymentCardRepository;
    private readonly IRideRepository _rideRepository;
    private readonly IPaymentDetailRepository _paymentDetailRepository;

    public PaystackWebhookCommandHandler(ITransactionLogRepository transactionLogRepository,
        IRiderRepository riderRepository, IRiderTransactionHistoryRepository riderTransactionHistoryRepository,
        IDriverRepository driverRepository, IDriverTransactionHistoryRepository driverTransactionHistoryRepository,
        IRiderWalletRepository riderWalletRepository, IDriverWalletRepository driverWalletRepository,
        IUnitOfWork unitOfWork, ITransactionReferenceMapRepository transactionReferenceMapRepository,
        IPaymentRepository paymentRepository, IPaymentCardRepository paymentCardRepository,
        IRideRepository rideRepository, IPaymentDetailRepository paymentDetailRepository)
    {
        _transactionLogRepository = transactionLogRepository;
        _riderRepository = riderRepository;
        _riderTransactionHistoryRepository = riderTransactionHistoryRepository;
        _driverRepository = driverRepository;
        _driverTransactionHistoryRepository = driverTransactionHistoryRepository;
        _riderWalletRepository = riderWalletRepository;
        _driverWalletRepository = driverWalletRepository;
        _unitOfWork = unitOfWork;
        _transactionReferenceMapRepository = transactionReferenceMapRepository;
        _paymentRepository = paymentRepository;
        _paymentCardRepository = paymentCardRepository;
        _rideRepository = rideRepository;
        _paymentDetailRepository = paymentDetailRepository;
    }

    public async Task<Result<bool>> Handle(PaystackWebhookCommand request, CancellationToken cancellationToken)
    {
        string requestContent = JsonSerializer.Serialize(request.RawData);

        TransactionLog transactionLog = new(Ulid.Parse(request.Reference), requestContent, 
            TransactionLogEvent.PaystackGeneralEvent);

        await _transactionLogRepository.AddAsync(transactionLog);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue(() => HandleEvent(request));

        return true;
    }

    public async Task HandleEvent(PaystackWebhookCommand request)
    {
        string requestEvent = request.Event;

        int amountInNaira = request.Amount / 100;

        if (requestEvent == "transfer.success")
            await HandleTransferSuccess(request, amountInNaira);

        else if (requestEvent == "transfer.reversed" || request.Event == "transfer.failed")
            await HandleTransferFailure(request, amountInNaira);

        else if (request.Event == "charge.success" && request.Status == "success")
            await HandleChargeSuccess(request, amountInNaira);

        else if (request.Event == "refund.processed" && request.Status == "processed")
            await HandleRefundSuccess(request, amountInNaira);

        else if (request.Event == "refund.failed")
            await HandleRefundFailure(request, amountInNaira);
    }

    private async Task HandleRefundFailure(PaystackWebhookCommand request, int amount)
    {
        var riderTransaction = await _riderTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (riderTransaction is null) return;

        riderTransaction.UpdateStatus(TransactionStatus.Failed);

        _riderTransactionHistoryRepository.Update(riderTransaction);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleRefundSuccess(PaystackWebhookCommand request, int amount)
    {
        var riderTransaction = await _riderTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference), RiderTransactionType.Refund);

        if (riderTransaction is null) return;

        riderTransaction.UpdateStatus(TransactionStatus.Success);

        _riderTransactionHistoryRepository.Update(riderTransaction);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleTransferSuccess(PaystackWebhookCommand request, int amount)
    {
        var transaction = await _driverTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (transaction is null) return;

        if (request.Status == "success")
        {
            transaction.UpdateStatus(TransactionStatus.Success);

            _driverTransactionHistoryRepository.Update(transaction);

            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task HandleTransferFailure(PaystackWebhookCommand request, int amountInNaira)
    {
        var transaction = await _driverTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (transaction is null)
        {
            var content = new PaystackEventWithdrawalProcess("Transaction was null from database", amountInNaira);

            TransactionLog transactionTransferLog = new(Ulid.Parse(request.Reference), 
                JsonSerializer.Serialize(content),
                TransactionLogEvent.PaystackWithdrawalEvent);

            await _transactionLogRepository.AddAsync(transactionTransferLog);

            await _unitOfWork.SaveChangesAsync();

            return;
        }

        transaction.UpdateStatus(TransactionStatus.Failed, 
            "Paystack webhook handler received a failed result for this transaction");

        var driver = await _driverRepository
            .GetAsync(transaction.DriverId);

        if (driver is null)
        {
            var content = new PaystackEventWithdrawalProcess($"Driver with Id:{transaction.DriverId} was null from database", 
                amountInNaira);

            TransactionLog transactionTransferLog = new(Ulid.Parse(request.Reference),
                JsonSerializer.Serialize(content),
                TransactionLogEvent.PaystackWithdrawalEvent);

            await _transactionLogRepository.AddAsync(transactionTransferLog);

            await _unitOfWork.SaveChangesAsync();

            return;
        }

        var driverWallet = await _driverWalletRepository.GetByDriverAsync(driver.Id);

        if (driverWallet is null)
        {
            var content = new PaystackEventWithdrawalProcess($"Wallet of driver with Id: {driver.Id} was null from database", 
                amountInNaira);

            TransactionLog transactionChargeLog = new(Ulid.Parse(request.Reference),
                JsonSerializer.Serialize(content),
                TransactionLogEvent.PaystackWithdrawalEvent);

            await _transactionLogRepository.AddAsync(transactionChargeLog);

            await _unitOfWork.SaveChangesAsync();

            return;
        }

        driverWallet.IncremementBalance(amountInNaira);

        _driverWalletRepository.Update(driverWallet);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task HandleChargeSuccess(PaystackWebhookCommand request, int amount)
    {
        var transactionMap = await _transactionReferenceMapRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (transactionMap is null) return;

        if(transactionMap.Type == TransactionReferenceType.RidePayment)
        {
            await ProcessRidePayment(request, amount);

            return;
        }
        else if(transactionMap.Type == TransactionReferenceType.RiderTransaction)
        {
            await ProcessRiderTransaction(request, amount);

            return;
        }
        else if(transactionMap.Type == TransactionReferenceType.DriverTransaction)
        {
            await ProcessDriverTransaction(request, amount);

            return;
        }       
    }

    private async Task ProcessRidePayment(PaystackWebhookCommand request, int amount)
    {
        //var ridePayment = await _paymentRepository
        //    .GetByReferenceAsync(Ulid.Parse(request.Reference));

        //if (ridePayment is null) return;

        var riderTransaction = await _riderTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if(riderTransaction is null)
        {
            // log

            return;
        }

        riderTransaction.UpdateStatus(TransactionStatus.Success);

        _riderTransactionHistoryRepository.Update(riderTransaction);

        await _unitOfWork.SaveChangesAsync();

        if (!riderTransaction.RideId.HasValue)
        {
            // log

            return;
        }

        var ride = await _rideRepository.GetAsync(riderTransaction.RideId.Value);

        if(ride is null)
        {
            // log

            return;
        }

        // ensure all ride payment has payment references tied to it...once it is successful here or verified using the 
        // rider tranaction reference, we update all the payment references tied to it...
        List<Ulid> paymentReferences = riderTransaction.RidePaymentReferences
            .Split(".")
            .Select(Ulid.Parse)
            .ToList();

        var paymentDetails = await _paymentDetailRepository.GetAllByReference(paymentReferences);

        foreach(var payment in paymentDetails)
        {
            payment.UpdateStatus(PaymentStatus.Success);
            payment.IncreaseOrOverrideAmountDue(0, true);
        }

        _paymentDetailRepository.UpdateRange(paymentDetails);

        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ProcessRiderTransaction(PaystackWebhookCommand request, int amount)
    {
        var riderTransaction = await _riderTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (riderTransaction is null) return;

        if(riderTransaction.Type == Domain.Riders.RiderTransactionType.FundWallet)
        {
            var wallet = await _riderWalletRepository
                .GetByRiderAsync(riderTransaction.RiderId);

            wallet!.IncrementBalance(amount);

            _riderWalletRepository.Update(wallet);

            riderTransaction.UpdateStatus(TransactionStatus.Success);

            _riderTransactionHistoryRepository.Update(riderTransaction);

            await _unitOfWork.SaveChangesAsync();

            return;
        }

        else if(riderTransaction.Type == Domain.Riders.RiderTransactionType.CardAddition)
        {
            await AddDebitCardAsync(riderTransaction.RiderId, request, amount);

            riderTransaction.UpdateStatus(TransactionStatus.Success);

            await _unitOfWork.SaveChangesAsync();

            return;
        }
    }

    private async Task ProcessDriverTransaction(PaystackWebhookCommand request, int amount)
    {
        var driverTransaction = await _driverTransactionHistoryRepository
            .GetByReferenceAsync(Ulid.Parse(request.Reference));

        if (driverTransaction is null) return;

        if (driverTransaction.Type == Domain.Drivers.DriverTransactionType.FundWallet)
        {
            var wallet = await _driverWalletRepository
                .GetByDriverAsync(driverTransaction.DriverId);

            wallet!.IncremementBalance(amount);

            _driverWalletRepository.Update(wallet);

            driverTransaction.UpdateStatus(TransactionStatus.Success);

            _driverTransactionHistoryRepository.Update(driverTransaction);

            await _unitOfWork.SaveChangesAsync();

            return;
        }
    }

    private async Task AddDebitCardAsync(long riderId, PaystackWebhookCommand request, int amount)
    {
        var rider = await _riderRepository
            .GetAsync(riderId);

        if (rider is null) return;

        PaymentCard paymentCard = new(rider.Id,
            request.AuthorizationCode,
            request.Last4,
            request.Bank,
            HandlePaymentCardType(request.CardType),
            request.ExpiryMonth,
            request.ExpiryYear,
            request.CustomerEmail,
            request.Signature);

        await _paymentCardRepository.AddAsync(paymentCard);

        var wallet = await _riderWalletRepository
              .GetByRiderAsync(rider.Id);

        wallet!.IncrementBalance(amount);

        _riderWalletRepository.Update(wallet);

        //string riderWebSocketKey = WebSocketKeys.Rider.Key(rider.Id.ToString());

        //var wsMessage = new WebSocketResponse<object>
        //{
        //    Event = "RIDER.CARDVERIFIED",
        //    Payload = new { message = "Payment card verified", success = true }
        //};

        //string message = JsonSerializer.Serialize(wsMessage);

        //await webSocketManager.SendMessageAsync(riderWebSocketKey, message);
    }

    private PaymentCardType HandlePaymentCardType(string paymentCardType)
    {
        if (paymentCardType.Contains("visa"))
            return PaymentCardType.Visa;

        else if (paymentCardType.Contains("master"))
            return PaymentCardType.MasterCard;

        else if (paymentCardType.Contains("verve"))
            return PaymentCardType.Verve;

        return PaymentCardType.Unknown;
    }

    internal sealed record PaystackEventWithdrawalProcess(string Message, int Amount);
}
