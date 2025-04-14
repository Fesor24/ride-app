namespace Ridely.Domain.Rides;
public class PricingService
{
    public long ConvertPriceInDecimalToLong(decimal amount, decimal discount)
    {
        decimal discountFigure = amount * (discount / 100.0m);

        amount -= discountFigure;

        decimal roundedValue = Math.Round(amount / 10.0m, 0) * 10;

        return long.Parse(roundedValue.ToString());
    }
    public long ConvertPriceInDecimalToLong(decimal amount)
    {
        decimal roundedValue = Math.Round(amount / 10.0m, 0) * 10;

        return long.Parse(roundedValue.ToString());
    }
}
