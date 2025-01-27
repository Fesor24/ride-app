namespace Soloride.Application.Features.Drivers.GetBankAcounts;
public sealed class GetBankAccountsResponse
{
    public long Id { get; set; }
    public string BankName { get; set; }
    public string AccountNo { get; set; }
    public string AccountName { get; set; }
}
