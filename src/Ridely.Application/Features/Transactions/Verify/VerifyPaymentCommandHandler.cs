using System.Text.Json;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Websocket;
using Ridely.Application.Models.Payment;
using Ridely.Application.Models.WebSocket;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Transactions;
using Ridely.Shared.Helper;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Transactions.Verify;
internal sealed class VerifyPaymentCommandHandler(IPaystackService paystackService,
    IUnitOfWork unitOfWork, IWebSocketManager webSocketManager) :
    ICommandHandler<VerifyPaymentCommand>
{
    public async Task<Result<bool>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        //var verificationResponse = await paystackService.VerifyAsync(request.Reference);

        //TransactionHistory? transactionHistory = await unitOfWork.TransactionHistoryRepository
        //    .GetByReferenceAsync(request.Reference);

        //if (transactionHistory is null) return false;

        //if (verificationResponse.IsFailure)
        //{
        //    transactionHistory.Status = TransactionStatus.Failed;

        //    unitOfWork.TransactionHistoryRepository.Update(transactionHistory);

        //    await unitOfWork.Complete();

        //    if (transactionHistory.Type == TransactionType.CardAddition)
        //    {
        //        string riderWebSocketKey = WebSocketKeys.Rider.Key(transactionHistory.RiderId!.Value.ToString());

        //        var wsMessage = new WebSocketResponse<object>
        //        {
        //            Event = "RIDER.CARDVERIFIED",
        //            Payload = new { message = "Error occurred during card verification", success = false }
        //        };

        //        string message = JsonSerializer.Serialize(wsMessage);

        //        await webSocketManager.SendMessageAsync(riderWebSocketKey, message);
        //    }

        //    return false;
        //}

        //transactionHistory.Status = TransactionStatus.Success;

        //unitOfWork.TransactionHistoryRepository.Update(transactionHistory);

        //await unitOfWork.Complete();

        //if (transactionHistory.Type == TransactionType.CardAddition)
        //    await ProcessCardAdditionSuccessPayment(transactionHistory, verificationResponse.Value);

        //else if (transactionHistory.Type == TransactionType.WalletCredit)
        //    await ProcessWalletTopupPayment(transactionHistory, verificationResponse.Value);

        return true;
    }

    //private async Task ProcessCardAdditionSuccessPayment(TransactionHistory transactionHistory,
    //    VerifyPaymentResponse paymentResponse)
    //{
    //    if (transactionHistory.RiderId is null) return;

    //    var rider = await unitOfWork.RiderRepository
    //        .GetAsync(transactionHistory.RiderId.Value);

    //    if (rider is null) return;

    //    PaymentCard paymentCard = new()
    //    {
    //        RiderId = rider.Id,
    //        AuthorizationCode = paymentResponse.Data.Authorization.AuthorizationCode,
    //        Last4Digits = paymentResponse.Data.Authorization.Last4,
    //        CardType = HandlePaymentCardType(paymentResponse.Data.Authorization.CardType),
    //        Bank = paymentResponse.Data.Authorization.Bank,
    //        ExpiryMonth = paymentResponse.Data.Authorization.ExpiryMonth,
    //        ExpiryYear = paymentResponse.Data.Authorization.ExpiryYear
    //    };

    //    await unitOfWork.PaymentCardRepository.AddAsync(paymentCard);

    //    var riderWallet = await unitOfWork.WalletRepository
    //        .GetAsync(rider.WalletId);

    //    if (riderWallet is null) return;

    //    riderWallet.AvailableBalance += 50;
    //    riderWallet.TotalBalance += 50;
    //    riderWallet.UpdatedAt = DateTime.UtcNow;

    //    TransactionHistory riderTransactionHistory = new()
    //    {
    //        Amount = 50,
    //        RiderId = rider.Id,
    //        Type = TransactionType.WalletCredit,
    //        Status = TransactionStatus.Success,
    //        Reference = RandomGenerator.GenerateTrxNumber()
    //    };

    //    transactionHistory.Status = TransactionStatus.Success;

    //    unitOfWork.WalletRepository.Update(riderWallet);

    //    await unitOfWork.TransactionHistoryRepository.AddAsync(riderTransactionHistory);

    //    await unitOfWork.Complete();

    //    string riderWebSocketKey = WebSocketKeys.Rider.Key(rider.Id.ToString());

    //    var wsMessage = new WebSocketResponse<object>
    //    {
    //        Event = "RIDER.CARDVERIFIED",
    //        Payload = new { message = "Payment card verified", success = true }
    //    };

    //    string message = JsonSerializer.Serialize(wsMessage);

    //    await webSocketManager.SendMessageAsync(riderWebSocketKey, message);
    //}

    //private async Task ProcessWalletTopupPayment(TransactionHistory transactionHistory,
    //    VerifyPaymentResponse paymentResponse)
    //{
    //    if (transactionHistory.RiderId.HasValue)
    //    {
    //        var rider = await unitOfWork.RiderRepository
    //            .GetAsync(transactionHistory.RiderId.Value);

    //        if (rider is null) return;

    //        var riderWallet = await unitOfWork.WalletRepository.GetAsync(rider.WalletId);

    //        if (riderWallet is null) return;

    //        riderWallet.AvailableBalance += paymentResponse.Data.AmountInNaira;
    //        riderWallet.TotalBalance += paymentResponse.Data.AmountInNaira;

    //        unitOfWork.WalletRepository.Update(riderWallet);

    //        await unitOfWork.Complete();
    //    }
    //    else if (transactionHistory.DriverId.HasValue)
    //    {
    //        var driver = await unitOfWork.DriverRepository
    //            .GetAsync(transactionHistory.DriverId.Value);

    //        if (driver is null) return;

    //        var driverWallet = await unitOfWork.WalletRepository.GetAsync(driver.WalletId);

    //        if (driverWallet is null) return;

    //        driverWallet.AvailableBalance += paymentResponse.Data.AmountInNaira;
    //        driverWallet.TotalBalance += paymentResponse.Data.AmountInNaira;

    //        unitOfWork.WalletRepository.Update(driverWallet);

    //        await unitOfWork.Complete();
    //    }
    //}

    //private PaymentCardType HandlePaymentCardType(string paymentCardType)
    //{
    //    if (paymentCardType.Contains("visa"))
    //        return PaymentCardType.Visa;

    //    else if (paymentCardType.Contains("master"))
    //        return PaymentCardType.MasterCard;

    //    else if (paymentCardType.Contains("verve"))
    //        return PaymentCardType.Verve;

    //    return PaymentCardType.Unknown;
    //}
}
