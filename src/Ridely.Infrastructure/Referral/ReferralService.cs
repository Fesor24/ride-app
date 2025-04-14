using Microsoft.EntityFrameworkCore;
using Ridely.Application.Abstractions.Referral;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;
using Ridely.Domain.Rides;

namespace Ridely.Infrastructure.Referral;
internal sealed class ReferralService : IReferralService
{
    private readonly ApplicationDbContext _context;

    public ReferralService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RewardsAfterRidersFirstCompletedRide(long riderId)
    {
        var rider = await _context.Set<Rider>()
            .FirstOrDefaultAsync(rider => rider.Id == riderId);

        if (rider is null) return;

        if (!rider.ReferredByUserId.HasValue) return;

        // no plan yet for rider referring rider....
        if (rider.ReferredByUser == Domain.Shared.ReferredUser.Rider) return;

        var riderCompletedRides = await _context.Set<Ride>()
            .Where(ride => ride.RiderId == riderId)
            .Where(ride => ride.Status == RideStatus.Completed || ride.Status == RideStatus.Reassigned)
            .OrderBy(ride => ride.CreatedAtUtc)
            .Take(2)
            .ToListAsync();

        // not riders first trip...
        if (riderCompletedRides.Count > 1) return;

        // rider was referred by a driver...
        var driver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(driver => driver.Id == rider.ReferredByUserId.Value);

        if (driver is null) return;

        var zeroCommissionDiscount = await _context.Set<DriverDiscount>()
            .Where(disc => disc.DriverId == driver.Id && disc.Type == DriverDiscountType.ZeroCommissionRide)
            .FirstOrDefaultAsync();

        if(zeroCommissionDiscount is null)
        {
            zeroCommissionDiscount = new(driver.Id);

            zeroCommissionDiscount.UpdateZeroCommissionDiscount();

            await _context.Set<DriverDiscount>().AddAsync(zeroCommissionDiscount);
        }
        else
        {
            zeroCommissionDiscount.UpdateZeroCommissionDiscount();

            _context.Set<DriverDiscount>().Update(zeroCommissionDiscount);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RewardsAfterDriversFirstCompletedRide(long driverId)
    {
        var driver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(driver => driver.Id == driverId);

        if (driver is null) return;

        if (!driver.ReferredByUserId.HasValue) return;

        // no plan yet for driver referring rider....
        if (driver.ReferredByUser == Domain.Shared.ReferredUser.Rider) return;

        var driverCompletedRides = await _context.Set<Ride>()
            .Where(ride => ride.DriverId == driver.Id)
            .Where(ride => ride.Status == RideStatus.Completed || ride.Status == RideStatus.Reassigned)
            .OrderBy(ride => ride.CreatedAtUtc)
            .Take(2)
            .ToListAsync();

        // not drivers first trip...
        if (driverCompletedRides.Count > 1) return;

        // driver was referred by a driver...
        var referredByDriver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(dr => dr.Id == driver.ReferredByUserId.Value);

        if (referredByDriver is null) return;

        var zeroCommissionDiscount = await _context.Set<DriverDiscount>()
            .Where(disc => disc.DriverId == referredByDriver.Id && disc.Type == DriverDiscountType.ZeroCommissionRide)
            .FirstOrDefaultAsync();

        if (zeroCommissionDiscount is null)
        {
            zeroCommissionDiscount = new(driver.Id);

            zeroCommissionDiscount.UpdateZeroCommissionDiscount();

            await _context.Set<DriverDiscount>().AddAsync(zeroCommissionDiscount);
        }
        else
        {
            zeroCommissionDiscount.UpdateZeroCommissionDiscount();

            _context.Set<DriverDiscount>().Update(zeroCommissionDiscount);
        }

        await _context.SaveChangesAsync();
    }
}
