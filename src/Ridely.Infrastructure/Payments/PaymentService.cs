using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Ridely.Application.Abstractions.Notifications;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Hubs;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;
using Ridely.Domain.Transactions;
using Ridely.Shared.Constants;
using Ridely.Shared.Helper.Keys;
using Ridely.Shared.SignalRCommunication;

namespace Ridely.Infrastructure.Payments;
internal sealed class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaystackService _paystackService;
    private readonly PricingService _pricingService;
    private readonly IHubContext<RideHub> _rideHubContext;
    private readonly IPushNotificationService _pushNotificationService;

    public PaymentService(ApplicationDbContext context, IPaystackService paystackService,
        PricingService pricingService, IHubContext<RideHub> rideHubContext, IPushNotificationService pushNotificationService)
    {
        _context = context;
        _paystackService = paystackService;
        _pricingService = pricingService;
        _rideHubContext = rideHubContext;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<Result> ProcessCardTripPaymentAsync(Ride ride, long amount, CancellationToken cancellationToken = default)
    {
        PaymentDetail? paymentDetail = await _context.Set<PaymentDetail>()
            .Where(detail => detail.PaymentId == ride.PaymentId && detail.PaymentFor == PaymentFor.EstimatedCharge)
            .FirstOrDefaultAsync();

        PaymentDetail? waitingChargeDetail = await _context.Set<PaymentDetail>()
            .Where(detail => detail.PaymentId == ride.PaymentId && detail.PaymentFor == PaymentFor.WaitTime)
            .FirstOrDefaultAsync();

        long chargeAmount;
        List<string> ridePaymentReferences = [];

        if(paymentDetail is null)
        {
            // todo: handle situation
            return Result.Success();
        }

        chargeAmount = paymentDetail.AmountDue;
        ridePaymentReferences.Add(paymentDetail.Reference.ToString());

        if (waitingChargeDetail is not null)
        {
            chargeAmount += waitingChargeDetail.AmountDue;
            ridePaymentReferences.Add(waitingChargeDetail.Reference.ToString());

            // todo: get the time requested for...extension
            // get when the driver arrived and ride started
            // calculate the time difference...
            // offset by time requested...
            // charge remainder using the per minute rate...
        }

        return await ChargeCardAsync(ride, paymentDetail, chargeAmount, ridePaymentReferences, cancellationToken);
    }

    public async Task<PaymentResponse> ProcessPaymentAndDriverCommissionAsync(Driver driver, Ride ride, 
        CancellationToken cancellationToken = default)
    {
        var paymentDetails = await _context.Set<PaymentDetail>()
            .Where(detail => detail.PaymentId == ride.PaymentId)
            .ToListAsync();

        if (ride.Payment.Method == PaymentMethod.Cash)
        {
            // todo: calculate the amount based on the payment detail
            // calculate extra waiting time also...
            // add all

            foreach (var detail in paymentDetails)
                detail.UpdateStatus(PaymentStatus.Success);

            _context.Set<PaymentDetail>().UpdateRange(paymentDetails);
        }

        var driverWallet = await _context.Set<DriverWallet>()
            .FirstOrDefaultAsync(wallet => wallet.DriverId == driver.Id);

        // next: wallet to be introduced in v2...
        //var riderWallet = await _context.Set<RiderWallet>()
        //    .FirstOrDefaultAsync(wallet => wallet.RiderId == ride.RiderId);

        var driverCommissionFromRide = await _context.Set<Settings>()
            .Select(setting => setting.DriverCommissionFromRide)
            .FirstOrDefaultAsync();

        var driverZeroRideCommission = await _context.Set<DriverDiscount>()
            .Where(disc => disc.DriverId == driver.Id && disc.Type == DriverDiscountType.ZeroCommissionRide)
            .FirstOrDefaultAsync();

        if(driverZeroRideCommission is not null && driverZeroRideCommission.ExpiredAtUtc >= DateTime.UtcNow)
        {
            driverCommissionFromRide = 100.0m;
        }

        long rideDiscountAmount = 0;

        var initialCharge = paymentDetails.First(detail => detail.PaymentFor == PaymentFor.EstimatedCharge).Amount;

        if (ride.Payment.DiscountInPercent > 0)
        {
            rideDiscountAmount = (long)(ride.Payment.DiscountInPercent / 100.0m) * initialCharge;
        }

        var waitingTimePaymentDetail = paymentDetails.FirstOrDefault(detail => detail.PaymentFor == PaymentFor.WaitTime);

        long waitingTimeAmount = waitingTimePaymentDetail is null ? 0 : waitingTimePaymentDetail.Amount;

        // inspect ride amount...
        long rideAmount = (initialCharge - rideDiscountAmount) + waitingTimeAmount;

        long fullRideAmount = paymentDetails.Sum(detail => detail.Amount);

        ride.Payment.UpdateAmount(fullRideAmount);

        _context.Set<Payment>().Update(ride.Payment);
        
        var driverCommission = (driverCommissionFromRide / 100.0m) * fullRideAmount;

        driverWallet!.IncremementBalance(driverCommission);

        _context.Set<DriverWallet>().Update(driverWallet);

        DriverTransactionHistory driverTransactionHistory = new(
            driver.Id,
            driverCommission,
            DriverTransactionType.RidePayment,
            Ulid.NewUlid(),
            TransactionStatus.Success
            );

        driverTransactionHistory.SetRide(ride.Id);

        await _context.Set<DriverTransactionHistory>().AddAsync(driverTransactionHistory);

        await _context.SaveChangesAsync(cancellationToken);

        //if(riderWallet is not null && riderWallet.AvailableBalance > 0)
        //{
        //    var amountDueByRider = rideAmount - riderWallet.AvailableBalance;

        //    // there was a reduction...
        //    if(amountDueByRider != rideAmount)
        //    {
        //        // the reason why amount due by rider will be less than 0, is if there's enough money in wallet to offset it...
        //        if (amountDueByRider < 0) amountDueByRider = 0;

        //        var amountToDeductFromWallet = rideAmount - amountDueByRider;

        //        riderWallet.DeductBalance(amountToDeductFromWallet);

        //        _context.Set<RiderWallet>().Update(riderWallet);

        //        RiderTransactionHistory riderTransactionHistory = new(
        //            ride.RiderId, 
        //            amountToDeductFromWallet, 
        //            RiderTransactionType.RidePaymentFromVirtualWallet, 
        //            Ulid.NewUlid(), 
        //            TransactionStatus.Success);

        //        riderTransactionHistory.SetRide(ride.Id);

        //        await _context.Set<RiderTransactionHistory>().AddAsync(riderTransactionHistory, cancellationToken);

        //        await _context.SaveChangesAsync();
        //    }
        //    return new PaymentResponse(fullRideAmount, (long)amountDueByRider, rideDiscountAmount);
        //}

        // todo: get the waiting time charge in total and replace with 0
        return new PaymentResponse(fullRideAmount, rideAmount, rideDiscountAmount, 0);
    }

    public async Task ProcessReroutedTripPaymentAsync(Ride ride, long completedDistanceFare, long remainderDistanceFare)
    {
        long newFare = completedDistanceFare + remainderDistanceFare;

        List<PaymentDetail> paymentDetails = [];

        var estimatedChargeDetail = await _context.Set<PaymentDetail>()
            .Where(detail => detail.PaymentFor == PaymentFor.EstimatedCharge && detail.PaymentId == ride.PaymentId)
            .FirstAsync();

        PaymentDetail rerouteChargeDetail = new(Ulid.NewUlid(), ride.PaymentId,
            PaymentFor.RerouteCharge, remainderDistanceFare);

        if (completedDistanceFare > estimatedChargeDetail.Amount)
        {
            estimatedChargeDetail.IncreaseOrOverrideAmountDue(completedDistanceFare - estimatedChargeDetail.Amount);
        }
        else if(completedDistanceFare < estimatedChargeDetail.Amount && estimatedChargeDetail.PaymentStatus == PaymentStatus.Success)
        {
            estimatedChargeDetail.IncreaseOrOverrideAmountDue(0, true);

            long credit = estimatedChargeDetail.Amount - completedDistanceFare;

            estimatedChargeDetail.UpdateCreditAmount(credit);

            rerouteChargeDetail.IncreaseOrOverrideAmountDue(remainderDistanceFare - credit);
        }

        if (estimatedChargeDetail.AmountDue > 0 && estimatedChargeDetail.PaymentStatus != PaymentStatus.Success)
            paymentDetails.Add(estimatedChargeDetail);

        paymentDetails.Add(rerouteChargeDetail);

        List<string> paymentReferences = paymentDetails
            .Select(detail => detail.Reference.ToString())
            .ToList();

        await _context.SaveChangesAsync();

        if (ride.Payment.Method == PaymentMethod.Cash) return;

        // proceed to charge card...
        var amountDue = paymentDetails.Sum(detail => detail.AmountDue);

        // todo: if successful for this transaction...we update all details for reroute
        var chargeResult = await ChargeCardAsync(ride, rerouteChargeDetail, amountDue, paymentReferences);

        if (chargeResult.IsSuccessful) return;

        await UpdatePaymentMethodToCashAndNotifyUsersAsync(ride);
        return;
    }

    public async Task RefundAsync(Ride ride, Ulid reference)
    {
        RiderTransactionHistory riderTransactionHistory = new(ride.RiderId,
            ride.EstimatedFare, 
            RiderTransactionType.Refund, 
            reference, TransactionStatus.Pending, PaymentProvider.Paystack);

        // todo: confirm...i think it must be the reference of the transaction to be refunded...
        // we do not need to pass any amount and the full amount will be refunded...we could do this...
        var result = await _paystackService.Refund(reference.ToString(), (int)ride.EstimatedFare);

        if (result.IsSuccessful)
        {
            await _context.Set<RiderTransactionHistory>().AddAsync(riderTransactionHistory);

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePaymentMethodToCashAndNotifyUsersAsync(Ride rideDetails)
    {
        if (rideDetails is null)
        {
            Console.WriteLine($"Ride null from {nameof(UpdatePaymentMethodToCashAndNotifyUsersAsync)} method");
            return;
        }

        string driverDeviceTokenId = rideDetails.Driver.DeviceTokenId ?? "";
        string riderDeviceTokenId = rideDetails.Rider.DeviceTokenId ?? "";

        string riderIdentifier = RiderKey.CustomNameIdentifier(rideDetails!.RiderId);
        string driverIdentifier = DriverKey.CustomNameIdentifier(rideDetails.DriverId!.Value);

        await _rideHubContext.Clients.Users(riderIdentifier, driverIdentifier)
            .SendAsync(SignalRSubscription.ReceivePaymentUpdates, new
            {
                Message = "An error occurred while processing payment for this ride",
            });

        if (!string.IsNullOrWhiteSpace(driverDeviceTokenId))
        {
            Dictionary<string, string> data = new()
            {

            };

            await _pushNotificationService.PushAsync(
                driverDeviceTokenId,
                "Card payment failed",
                "Payment updated to cash",
                data, PushNotificationType.CardTripPaymentFailure);
        }

        if (!string.IsNullOrWhiteSpace(riderDeviceTokenId))
        {
            Dictionary<string, string> data = new()
            {

            };

            await _pushNotificationService.PushAsync(
                riderDeviceTokenId,
                "Card payment failed",
                "Payment updated to cash",
                data, PushNotificationType.CardTripPaymentFailure);
        }

        rideDetails.Payment.UpdatePaymentMethod(PaymentMethod.Cash);

        _context.Set<Ride>().Update(rideDetails);

        await _context.SaveChangesAsync();
    }

    public async Task VerifyCardPaymentAndUpdatePaymentMethodToCashIfFailAsync(Ride ride)
    {
        if (ride is null)
        {
            Console.WriteLine($"Ride null from {nameof(VerifyCardPaymentAndUpdatePaymentMethodToCashIfFailAsync)} method");
            return;
        }

        if (ride.Payment.Method == PaymentMethod.Cash) return;

        var estimatedChargePaymentDetail = await _context.Set<PaymentDetail>()
            .Where(detail => detail.PaymentId == ride.PaymentId && detail.PaymentFor == PaymentFor.EstimatedCharge)
            .FirstAsync();

        if (estimatedChargePaymentDetail.PaymentStatus == PaymentStatus.Success) return;

        var verificationResult = await _paystackService.VerifyAsync(estimatedChargePaymentDetail.Reference.ToString());

        if (verificationResult.IsSuccessful && verificationResult.Value.Status &&
            verificationResult.Value.Data.Status == "success")
        {
            estimatedChargePaymentDetail.UpdateStatus(PaymentStatus.Success);

            _context.Set<PaymentDetail>().Update(estimatedChargePaymentDetail);

            await _context.SaveChangesAsync();

            return;
        }

        await UpdatePaymentMethodToCashAndNotifyUsersAsync(ride);
    }

    private async Task<Result> ChargeCardAsync(Ride ride, PaymentDetail paymentDetail, long amount, 
        List<string> ridePaymentReferences, CancellationToken cancellationToken = default)
    {
        if (amount == 0)
            amount = paymentDetail.Amount;

        if (amount == 0) return Result.Success();

        if (!ride.Payment.PaymentCardId.HasValue)
        {
            var failureContent = new PaymentFailure(
                "No payment card id associated with ride payment",
                "Ride has payment method of card but no payment card id was detected");

            paymentDetail.UpdateStatus(PaymentStatus.Failed, failureContent.ToString());

            paymentDetail.UpdateStatus(PaymentStatus.Failed, failureContent.ToString());

            _context.Set<PaymentDetail>().Update(paymentDetail);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("payment.failure",
                "No payment card found to process payment"));
        }

        PaymentCard? paymentCard = await _context.Set<PaymentCard>()
            .FirstOrDefaultAsync(x => x.Id == ride.Payment.PaymentCardId.Value);

        if (paymentCard is null)
        {
            var failureContent = new PaymentFailure(
                "Payment card with id not found",
                "Ride has payment method of card but no payment card was found");

            paymentDetail.UpdateStatus(PaymentStatus.Failed, failureContent.ToString());

            _context.Set<PaymentDetail>().Update(paymentDetail);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("payment.failure",
                "No payment card found to process payment"));
        }

        Ulid riderTransactionReference = Ulid.NewUlid();

        RiderTransactionHistory riderTransactionHistory = new(ride.RiderId,
              ride.EstimatedFare, RiderTransactionType.RidePayment,
              riderTransactionReference, TransactionStatus.Pending, 
              PaymentProvider.Paystack, ride.Id);

        riderTransactionHistory.SetRidePaymentReference(ridePaymentReferences);

        var res = await _paystackService.ChargeAsync(paymentCard.AuthorizationCode,
                paymentCard.Email, (int)amount,
                riderTransactionReference.ToString());

        if (res.IsSuccessful)
        {
            await _context.Set<RiderTransactionHistory>().
                AddAsync(riderTransactionHistory);

            TransactionReferenceMap transactionReferenceMap = new(
                riderTransactionReference,
                TransactionReferenceType.RidePayment);

            await _context.Set<TransactionReferenceMap>()
                .AddAsync(transactionReferenceMap);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        else
        {
            var failureResult = new PaymentFailure("Error from paystack service charge method",
                JsonSerializer.Serialize(res.Error));

            paymentDetail.UpdateStatus(PaymentStatus.Failed, failureResult.ToString());

            _context.Set<PaymentDetail>().Update(paymentDetail);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("payment.failure",
                "Error while processing payment"));
        }
    }  

    internal sealed record PaymentFailure(string Message, string Details)
    {
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
