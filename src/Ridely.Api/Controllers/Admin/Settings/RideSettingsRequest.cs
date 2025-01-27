namespace SolorideAPI.Controllers.Admin.Settings;

public class RideSettingsRequest
{
    public decimal BaseFare { get; set; }
    public decimal RatePerKilometer { get; set; }
    public decimal DeliveryRatePerKilometer { get; set; }
    public decimal DriverCommission { get; set; }
    public decimal RatePerMinute { get; set; }
    public int PremiumCab { get; set; }
}
