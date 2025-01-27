using System.Text.Json;

namespace Soloride.Domain.Models.Payments;
public class BankAccountModel
{
    public string BankName { get; set; }
    public string AccountNo { get; set; }
    public string AccountName { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
