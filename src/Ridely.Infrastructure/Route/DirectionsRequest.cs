namespace Ridely.Infrastructure.Route;
// MapBox
public class DirectionsRequest
{
    public DirectionOrigin Origin { get; set; }
    public DirectionDestination Destination { get; set; }

    public class DirectionOrigin
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }

    public class DirectionDestination
    {
        public double Lat { get; set; }
        public double Long { get; set; }
    }
}

// Google
public class RouteRequest
{
    public RouteRequestLocation Origin { get; set; }
    public RouteRequestLocation Destination { get; set; }
    public List<RouteRequestLocation>? Intermediates { get; set; }
    public string TravelMode { get; set; } = "DRIVE";
    public string RoutingPreference { get; set; } = "TRAFFIC_AWARE";
    public bool ComputeAlternativeRoutes { get; set; } = false;
    public string LanguageCode { get; set; } = "en-US";
    public string Units { get; set; } = "IMPERIAL";

    public class RouteRequestLocation
    {
        public RequestLocation Location { get; set; }
        public class RequestLocation
        {
            public Coordinates LatLng { get; set; }

            public class Coordinates
            {
                public double Latitude { get; set; }
                public double Longitude { get; set; }
            }
        }
    }
}
