namespace Ridely.Domain.Models;
public abstract class SearchParams
{
    private int _pageSize = 10;

    private const int MaxPageSize = 100;

    public int PageSize { get => _pageSize; set => _pageSize = value > MaxPageSize ? MaxPageSize : value; }
    public int PageNumber { get;set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
}
