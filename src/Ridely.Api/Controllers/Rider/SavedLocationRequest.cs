using Ridely.Domain.Models;
using Ridely.Domain.Riders;

namespace RidelyAPI.Controllers.Rider;

public class SavedLocationRequest
{
    public SavedLocationType LocationType { get; set; }
    public Location Coordinates { get; set; }
    public string Address { get; set; }
}

public class UpdateSavedLocationRequest
{
    public long SavedLocationId { get; set; }
    public Location Coordinates { get; set; }
    public string Address { get; set; }
}
