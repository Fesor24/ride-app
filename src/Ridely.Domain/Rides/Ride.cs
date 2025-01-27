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
    public string WayPointAddresses { get; private set; } = "";
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
    public string? CancellationReason { get; private set; }
    public string? ReassignReason { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public ICollection<Chat> Chats { get; private set; } = [];
    public ICollection<CallLog> CallLogs { get; private set; } = [];
    public ICollection<RideLog> RideLogs { get; private set; } = [];

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

    public void UpdateWaypointCoordinates(double latitude, double longitude)
    {
        if (string.IsNullOrWhiteSpace(WaypointCordinates))
        {
            List<Location> locations = [];

            Location coordinates = new()
            {
                Latitude = latitude,
                Longitude = longitude
            };

            locations.Add(coordinates);

            WaypointCordinates = JsonSerializer.Serialize(locations);
        }
        else
        {
            try
            {
                List<Location> locations = JsonSerializer.Deserialize<List<Location>>(WaypointCordinates) ?? [];

                locations.Add(new Location { Latitude = latitude, Longitude = longitude });

                WaypointCordinates = JsonSerializer.Serialize(locations);
            }
            catch (Exception)
            {
                List<Location> locations = [
                        new Location{Latitude = latitude, Longitude = longitude}
                    ];

                WaypointCordinates = JsonSerializer.Serialize(locations);
            }
        }
    }

    public void UpdateWaypointAddresses(string address)
    {
        WayPointAddresses += $"%%{address}";
    }

    public void UpdateStatus(
        RideStatus status, 
        long? driverId = null,
        string? cancellationReason = null,
        string? reassignReason = null)
    {
        Status = status;

        if(!string.IsNullOrWhiteSpace(cancellationReason))
            CancellationReason = cancellationReason;

        if(!string.IsNullOrWhiteSpace(reassignReason))
            ReassignReason = reassignReason;

        if(driverId.HasValue)
            DriverId = driverId.Value;
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
