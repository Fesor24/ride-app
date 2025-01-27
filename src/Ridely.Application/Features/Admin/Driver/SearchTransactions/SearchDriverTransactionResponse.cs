namespace Ridely.Application.Features.Admin.Driver.SearchTransactions;
public sealed class SearchDriverTransactionResponse
{
    public string Reference { get; set; }
    public string Driver { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}
