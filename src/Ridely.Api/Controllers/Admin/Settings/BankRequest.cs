namespace RidelyAPI.Controllers.Admin.Settings;

public class BankRequest
{
    public List<Bank> Banks { get; set; }
}

public class Bank
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Type { get; set; }
}
