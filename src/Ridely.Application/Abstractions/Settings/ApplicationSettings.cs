namespace Ridely.Application.Abstractions.Settings;
public class ApplicationSettings
{
    public int FreeWaitingTimeInMins { get; init; }
    public int ChargePerMinuteForWaiting { get; init; }
    public int MaximumFundDeficitFromDriver { get; init; }
    public int MinimumWithdrawAmount { get; init; }
    public int PaystackWithdrawCharge { get; init; }
}
