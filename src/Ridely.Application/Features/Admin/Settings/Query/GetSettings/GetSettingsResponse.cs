namespace Ridely.Application.Features.Admin.Settings.Query.GetSettings;
public class GetSettingsResponse
{
    public decimal BaseFare { get; set; }
    public decimal RatePerKilometer { get; set; }
    public decimal DeliveryRatePerKilometer { get; set; }
    public decimal DriverCommission { get; set; }
    public decimal RatePerMinute { get; set; }
    public int PremiumCab { get; set; }
}
