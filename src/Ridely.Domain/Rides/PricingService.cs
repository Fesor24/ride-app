namespace Soloride.Domain.Rides;
public class PricingService
{
    public long FormatPrice(decimal amount)
    {
        decimal roundedValue = Math.Round(amount / 10.0m, 0) * 10;

        return long.Parse(roundedValue.ToString());
    }
}
