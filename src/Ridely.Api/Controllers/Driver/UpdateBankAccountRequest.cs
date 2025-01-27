namespace SolorideAPI.Controllers.Driver;

public class UpdateBankAccountRequest
{
    public int BankId { get; set; }
    public string AccountNo { get; set; }
    public string Otp { get; set; }
}
