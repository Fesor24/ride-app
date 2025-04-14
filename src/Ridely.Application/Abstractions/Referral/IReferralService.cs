namespace Ridely.Application.Abstractions.Referral;
public interface IReferralService
{
    Task RewardsAfterRidersFirstCompletedRide(long riderId);
    Task RewardsAfterDriversFirstCompletedRide(long driverId);
}
