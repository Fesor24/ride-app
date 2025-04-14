using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Call;
using Ridely.Domain.Drivers;
using Ridely.Domain.Models;
using Ridely.Domain.Riders;

namespace Ridely.Domain.Rides;

public sealed class Ride : Entity
{
    private Ride()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long RiderId { get; private set; }
    [ForeignKey(nameof(RiderId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Rider Rider { get; private set; }
    public long? DriverId { get; private set; }
    [ForeignKey(nameof(DriverId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Driver Driver { get; private set; }
    public bool HaveConversation { get; private set; }
    public RideStatus Status { get; private set; }
    public MusicGenre MusicGenre { get; private set; }
    public long PaymentId { get; private set; }
    [ForeignKey(nameof(PaymentId))]
    public Payment Payment { get; private set; }
    public long EstimatedFare { get; private set; }
    public long EstimatedDeliveryFare { get; private set; }
    public string SourceAddress { get; private set; }
    public string WaypointAddresses { get; private set; } = "";
    public string DestinationAddress { get; private set; }
    public string SourceCordinates { get; private set; }
    public string WaypointCordinates { get; private set; } = "";
    public string DestinationCordinates { get; private set; }
    public double DistanceInMeters { get; private set; }
    public int EstimatedDurationInSeconds { get; private set; }
    public long? ReassignFromId { get; private set; }
    [ForeignKey(nameof(ReassignFromId))]
    public Ride ReassignFrom { get; private set; }
    public RideCategory Category { get; private set; }
    // reroute can only be done once...for now...
    public bool WasRerouted { get; private set; } = false;
    public string RerouteTo { get; private set; } = string.Empty;
    public string? CancellationReason { get; private set; }
    public UserType CancelledBy { get; private set; }
    public string? ReassignReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<Chat> Chats { get; private set; } = [];
    public ICollection<CallLog> CallLogs { get; private set; } = [];
    public ICollection<RideLog> RideLogs { get; private set; } = [];

    public void UpdateEstimatedFare(long fare)
    {
        EstimatedFare = fare;
    }

    private void UpdateCordinates(double sourceLat, double sourceLongitude,
        double destinationLat, double destinationLong)
    {
        SourceCordinates = $"{sourceLat}:{sourceLongitude}";
        DestinationCordinates = $"{destinationLat}:{destinationLong}";
    }

    public void UpdateDestinationCordinates(double destinationLat, double destinationLong)
    {
        DestinationCordinates = $"{destinationLat}:{destinationLong}";
    }

    public Location GetCoordinates(string coordinate)
    {
        string[] locationCordinateSplit = coordinate.Split(':');

        return new Location
        {
            Latitude = double.Parse(locationCordinateSplit[0]),
            Longitude = double.Parse(locationCordinateSplit[1])
        };
    }

    public List<Location> GetWaypointCoordinates()
    {
        if (string.IsNullOrWhiteSpace(WaypointCordinates)) return [];

        return JsonSerializer.Deserialize<List<Location>>(WaypointCordinates) ?? [];
    }

    public void UpdateWaypointCoordinates(double latitude, double longitude, bool rideExtension = false)
    {
        if (string.IsNullOrWhiteSpace(WaypointCordinates))
        {
            List<WaypointLocation> locations = [];

            WaypointLocation coordinates = new()
            {
                Latitude = latitude,
                Longitude = longitude,
                RideExtension = rideExtension
            };

            locations.Add(coordinates);

            WaypointCordinates = JsonSerializer.Serialize(locations);
        }
        else
        {
            try
            {
                List<WaypointLocation> locations = JsonSerializer.Deserialize<List<WaypointLocation>>(WaypointCordinates) ?? [];

                locations.Add(new WaypointLocation { Latitude = latitude, Longitude = longitude, RideExtension = rideExtension });

                WaypointCordinates = JsonSerializer.Serialize(locations);
            }
            catch (Exception)
            {
                List<WaypointLocation> locations = [
                        new WaypointLocation{Latitude = latitude, Longitude = longitude, RideExtension = rideExtension}
                    ];

                WaypointCordinates = JsonSerializer.Serialize(locations);
            }
        }
    }

    public void UpdateWaypointAddresses(string address)
    {
        if (string.IsNullOrWhiteSpace(WaypointAddresses))
            WaypointAddresses = address;

        else
            WaypointAddresses += $"%%{address}";
    }

    public void UpdateStatus(
        RideStatus status, 
        long? driverId = null,
        string? cancellationReason = null,
        string? reassignReason = null,
        UserType cancelledBy = UserType.Rider)
    {
        Status = status;

        if(!string.IsNullOrWhiteSpace(cancellationReason))
            CancellationReason = cancellationReason;

        if(!string.IsNullOrWhiteSpace(reassignReason))
            ReassignReason = reassignReason;

        if(driverId.HasValue)
            DriverId = driverId.Value;

        if (status == RideStatus.Cancelled)
            CancelledBy = cancelledBy;
    }

    public void Reroute(string address, double latitude, double longitude)
    {
        RerouteTo rerouteTo = new(address, latitude, longitude);

        RerouteTo = rerouteTo.ToString();
    }

    public void UpdateRideRequest(RideCategory rideCategory, bool? haveConversation = true, 
        MusicGenre musicGenre = MusicGenre.None)
    {
        Status = RideStatus.Requested;
        HaveConversation = haveConversation.HasValue ? haveConversation.Value : true;
        MusicGenre = musicGenre;
        Category = rideCategory;
    }

    public void UpdateDestinationAddress(string destinationAddress)
    {
        DestinationAddress = destinationAddress;
    }

    public static Ride CreateRide(long riderId, long estimatedFare,
        long estimatedDeliveryFare,
        double sourceLat, double sourceLong,
        double destinationLat, double destinationLong,
        long paymentId, double distanceInMeters,
        string sourceAddress, string destinationAddress,
        int durationInSeconds,
        bool haveConversation = true, 
        MusicGenre musicGenre = MusicGenre.None, RideStatus rideStatus = RideStatus.FareEstimate,
        long? reassignFromId = null)
    {
        Ride ride = new()
        {
            RiderId = riderId,
            EstimatedFare = estimatedFare,
            PaymentId = paymentId,
            HaveConversation = haveConversation,
            EstimatedDeliveryFare = estimatedDeliveryFare,
            Category = RideCategory.CabEconomy, // default category...
            DistanceInMeters = distanceInMeters,
            MusicGenre = musicGenre,
            Status = rideStatus,
            ReassignFromId = reassignFromId,
            SourceAddress = sourceAddress,
            DestinationAddress = destinationAddress,
            EstimatedDurationInSeconds = durationInSeconds,
        };

        ride.UpdateCordinates(sourceLat, sourceLong, destinationLat, destinationLong);

        return ride;
    }

    public static Ride CreateDelivery()
    {
        Ride ride = new()
        {
            Category = RideCategory.CabEconomy
        };

        return ride;
    }
}

internal sealed class WaypointLocation : Location
{
    public bool RideExtension { get; set; }
}

public sealed record RerouteTo(string Address, double Latitude, double Longitude)
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public static RerouteTo FromString(string rerouteInfo) => JsonSerializer.Deserialize<RerouteTo>(rerouteInfo)!;
}
