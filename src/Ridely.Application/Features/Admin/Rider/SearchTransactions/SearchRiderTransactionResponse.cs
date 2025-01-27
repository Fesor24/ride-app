namespace Ridely.Application.Features.Admin.Rider.SearchTransactions;
public sealed class SearchRiderTransactionResponse
{
    public string Reference { get; set; }
    public string Rider { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string CreatedAt { get; set; }
}
