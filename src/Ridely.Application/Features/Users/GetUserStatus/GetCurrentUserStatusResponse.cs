using Ridely.Domain.Models;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Users.GetUserStatus;
public sealed class GetCurrentUserStatusResponse
{
    public CurrentUserStatusEnum Status { get; set; }
    public CurrentUserRide? Ride { get; set; }

    public class CurrentUserRide
    {
        public long RideId { get; set; }
        public RideStatus RideStatus { get; set; }
        public string Source { get; set; } = "";
        public string Destination { get; set; } = "";
        public Location SourceCoordinates { get; set; } = new();
        public Location DestinationCoordinates { get; set; } = new();
        public PaymentMethod PaymentMethod { get; set; }
        public long FareEstimate { get; set; }
        public string Rider { get; set; } = "";
        public CurrentDriver Driver { get; set; } = new();

        public class CurrentDriver
        {
            public string FirstName { get; set; } = "";
            public string LastName { get; set; } = "";
            public string ProfileImageUrl { get; set; } = "";
            public DriverCab Cab { get; set; } = new();

            public class DriverCab
            {
                public string Color { get; set; }
                public string Manufacturer { get; set; }
                public string LicensePlateNo { get; set; }
                public string Model { get; set; }
            }
        }

        public string MusicGenre { get; set; } = "";
        public bool? RideConversation { get; set; }
    }
}

public enum CurrentUserStatusEnum
{
    Unknown = 0,
    Default = 1,
    Ride = 2
}
