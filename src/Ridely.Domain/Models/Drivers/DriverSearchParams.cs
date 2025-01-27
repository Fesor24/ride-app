using Ridely.Domain.Drivers;

namespace Ridely.Domain.Models.Drivers;
public class DriverSearchParams : SearchParams
{
    public CabType? CabType { get; set; }
    public string? PhoneNo { get; set; }
    public string? Email { get; set; }
}
