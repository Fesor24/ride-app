using Ridely.Domain.Models;

namespace Ridely.Application.Features.Riders.GetSavedLocations;
public sealed class GetSavedLocationResponse
{
    public long Id { get; set; }
    public string Address { get; set; }
    public Location Coordinates { get; set; }
}
