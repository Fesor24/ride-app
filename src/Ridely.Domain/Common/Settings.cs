using Soloride.Domain.Abstractions;

namespace Soloride.Domain.Common;
public sealed class Settings : Entity
{
    public decimal BaseFare { get; set; }
    public decimal RatePerKilometer { get; set; }
    public decimal DeliveryRatePerKilometer { get; set; }
    // 100% means driver gets paid all...if Soloride wants 30%, then driver commisison would be 70%
    public decimal DriverCommissionFromRide { get; set; }
    public decimal RatePerMinute { get; set; }
    public string SupportEmails { get; private set; } = "";
    public string SupportPhoneNo { get; private set; } = "";
    public string EmergencyLines { get; private set; } = "";
    public int PremiumCab { get; set; }

    //TODO: Integrate endpoint for this on admin
    public void SetSupportEmail(string email)
    {
        email = email.Replace(" ", "")
            .Replace(",", "")
            .Trim().ToLower();

        SupportEmails += $",{email}";
    }

    public void SetSupportPhoneNo(string phoneNo)
    {
        phoneNo = phoneNo.Replace(" ", "")
            .Replace(",", "")
            .Trim();

        SupportPhoneNo += $",{phoneNo}";
    }

    public void SetEmergencyLines(string emergencyLine)
    {
        emergencyLine = emergencyLine.Replace(" ", "")
            .Replace(",", "")
            .Trim();

        EmergencyLines += $",{emergencyLine}";
    }

    public List<string> GetSupportEmails()
    {
        return [.. SupportEmails.Split(',')];
    }

    public List<string> GetSupportPhoneNo()
    {
        return [.. SupportPhoneNo.Split(',')];
    }

    public List<string> GetEmergencyLines()
    {
        return [.. EmergencyLines.Split(',')];
    }
}
