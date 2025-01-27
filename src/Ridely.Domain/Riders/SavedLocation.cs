using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Models;

namespace Soloride.Domain.Riders;
public sealed class SavedLocation : Entity
{
    private SavedLocation()
    {

    }

    public SavedLocation(long riderId, SavedLocationType locationType,
        double lat, double longitude, string address)
    {
        RiderId = riderId;
        LocationType = locationType;
        Address = address;

        SetCordinates(lat, longitude);
    }

    public long RiderId { get; private set; }
    [ForeignKey(nameof(RiderId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Rider Rider { get; }
    public SavedLocationType LocationType { get; private set; }
    public string Coordinates { get; private set; }
    public string Address { get; private set; }

    public void SetCordinates(double lat, double longitude)
    {
        Coordinates = $"{lat}:{longitude}";
    }

    public Location GetCoordinates()
    {
        string[] coordinatesSplit = Coordinates.Split(':');

        return new Location
        {
            Latitude = double.Parse(coordinatesSplit[0]),
            Longitude = double.Parse(coordinatesSplit[1]),
        };
    }

    public void UpdateAddress(string address, double lat, double longitude)
    {
        Address = address;
        SetCordinates(lat, longitude);
    }
}
