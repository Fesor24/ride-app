namespace Soloride.Application.Abstractions.Referral;
public interface IReferralService
{
    Task RewardsAfterRidersFirstCompletedRide(long riderId);
}
