using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Soloride.Application.Abstractions.Payment;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Transactions;

namespace Soloride.Infrastructure.Payments;
internal sealed class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IPaystackService _paystackService;
    private readonly PricingService _pricingService;

    public PaymentService(ApplicationDbContext context, IPaystackService paystackService,
        PricingService pricingService)
    {
        _context = context;
        _paystackService = paystackService;
        _pricingService = pricingService;
    }

    public async Task<Result> ProcessCardTripPaymentAsync(Ride ride,
        CancellationToken cancellationToken = default)
    {
        return await ProcessPayment(ride, cancellationToken);
    }

    public async Task<(long RideFare, long AmountDueByRider)> ProcessPaymentAndDriverCommissionAsync(Driver driver, Ride ride, 
        CancellationToken cancellationToken = default)
    {
        // todo: consider if money was removed from users wallet...
        // users wallet is considered for cash trips at the end of the ride
        // or card trips if at the end of the ride there's a remainder to pay...

        // card payment has been processed and marked as success already...
        // todo: if amount after ride will be more than estimated fare and method is card
        // we have to process payment again from rider....we mark the payment as partial before processing it...
        if (ride.Payment.Method == PaymentMethod.Cash)
        {
            ride.Payment.UpdateStatus(PaymentStatus.Success);

            _context.Set<Payment>().Update(ride.Payment);
        }

        var driverWallet = await _context.Set<DriverWallet>()
            .FirstOrDefaultAsync(wallet => wallet.DriverId == driver.Id);

        var riderWallet = await _context.Set<RiderWallet>()
            .FirstOrDefaultAsync(wallet => wallet.RiderId == ride.RiderId);

        var driverCommissionFromRide = await _context.Set<Settings>()
            .Select(setting => setting.DriverCommissionFromRide)
            .FirstOrDefaultAsync();

        // todo: it estimated fare will be different from final payment, we replace this with the amount...
        long rideAmount = ride.EstimatedFare;
        
        var driverCommission = (driverCommissionFromRide / 100.0m) * rideAmount;

        driverWallet!.IncremementBalance(driverCommission);

        _context.Set<DriverWallet>().Update(driverWallet);

        DriverTransactionHistory driverTransactionHistory = new(
            driver.Id,
            driverCommission,
            DriverTransactionType.RidePayment,
            ride.Payment.Reference,
            TransactionStatus.Success
            );

        await _context.Set<DriverTransactionHistory>().AddAsync(driverTransactionHistory);

        await _context.SaveChangesAsync(cancellationToken);

        if(riderWallet is not null && riderWallet.AvailableBalance > 0)
        {
            var amountDueByRider = rideAmount - riderWallet.AvailableBalance;

            // there was a reduction...
            if(amountDueByRider != rideAmount)
            {
                if (amountDueByRider < 0) amountDueByRider = 0;

                var amountToDeductFromWallet = rideAmount - amountDueByRider;

                riderWallet.DeductBalance(amountToDeductFromWallet);

                _context.Set<RiderWallet>().Update(riderWallet);

                RiderTransactionHistory riderTransactionHistory = new(
                    ride.RiderId, 
                    amountToDeductFromWallet, 
                    RiderTransactionType.RidePaymentFromVirtualWallet, 
                    ride.Payment.Reference, 
                    TransactionStatus.Success);

                await _context.Set<RiderTransactionHistory>().AddAsync(riderTransactionHistory);

                await _context.SaveChangesAsync();
            }

            return (rideAmount, (long)amountDueByRider);
        }

        return (rideAmount, rideAmount);
    }

    public async Task RefundAsync(Ride ride, Ulid reference)
    {
        RiderTransactionHistory riderTransactionHistory = new(ride.RiderId,
            ride.EstimatedFare, 
            RiderTransactionType.Refund, 
            reference, TransactionStatus.Pending);

        // todo: confirm...i think it must be the reference of the transaction to be refunded...
        // we do not need to pass any amount and the full amount will be refunded...we could do this...
        var result = await _paystackService.Refund(reference.ToString(), (int)ride.EstimatedFare);

        if (result.IsSuccessful)
        {
            await _context.Set<RiderTransactionHistory>().AddAsync(riderTransactionHistory);

            await _context.SaveChangesAsync();
        }
    }

    private async Task<Result> ProcessPayment(Ride ride, CancellationToken cancellationToken = default)
    {
        if (!ride.Payment.PaymentCardId.HasValue)
        {
            var failureContent = new PaymentFailure(
                "No payment card id associated with ride payment",
                "Ride has payment method of card but no payment card id was detected");

            string error = JsonSerializer.Serialize(failureContent);

            ride.Payment.UpdateStatus(PaymentStatus.Failed, error);

            _context.Set<Payment>().Update(ride.Payment);

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

            string error = JsonSerializer.Serialize(failureContent);

            ride.Payment.UpdateStatus(PaymentStatus.Failed, error);

            _context.Set<Payment>().Update(ride.Payment);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("payment.failure",
                "No payment card found to process payment"));
        }

        var res = await _paystackService.ChargeAsync(paymentCard.AuthorizationCode,
                paymentCard.Email, (int)ride.EstimatedFare,
                ride.Payment.Reference.ToString());

        if (res.IsSuccessful)
        {
            RiderTransactionHistory riderTransactionHistory = new(ride.RiderId,
                ride.EstimatedFare, RiderTransactionType.RidePaymentUsingPaystack,
                Ulid.Parse(res.Value.Data.Reference), TransactionStatus.Pending);

            await _context.Set<RiderTransactionHistory>().
                AddAsync(riderTransactionHistory);

            TransactionReferenceMap transactionReferenceMap = new(
                Ulid.Parse(res.Value.Data.Reference),
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

            ride.Payment.UpdateStatus(PaymentStatus.Failed, JsonSerializer.Serialize(failureResult));

            _context.Set<Payment>().Update(ride.Payment);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Failure(new Error("payment.failure",
                "Error while processing payment"));
        }
    }  

    internal sealed record PaymentFailure(string Message, string Details);
}
