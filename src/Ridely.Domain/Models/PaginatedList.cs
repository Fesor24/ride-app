namespace Soloride.Domain.Models;
public class PaginatedList<T>
{
    public int TotalItems { get; set; }
    public List<T> Items { get; set; } = [];
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
