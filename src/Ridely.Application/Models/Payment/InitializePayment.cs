namespace Soloride.Application.Models.Payment;
public class InitializePayment
{
    public int Amount { get; set; }
    public string Email { get; set; }
    public bool CardVerification { get; set; } = false;
    public string Reference { get; set; }
}
