namespace Ridely.Domain.Models.Common;
public class SavedLocationModel : BaseModel
{
    public string Address { get; set; }
    public Location Coordinates { get; set; }
}
