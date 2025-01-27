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

        var driver = await _context.Set<Driver>()
            .FirstOrDefaultAsync(driver => driver.Id == rider.ReferredByUserId.Value);

        if (driver is null) return;

        driver.UpdateZeroCommissionRide();

        await _context.SaveChangesAsync();
    }
}
