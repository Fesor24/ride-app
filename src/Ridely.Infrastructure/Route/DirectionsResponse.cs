using System.Text.Json.Serialization;

namespace Ridely.Infrastructure.Route;
// Mapbox
public class DirectionsResponse
{
    [JsonPropertyName("waypoints")]
    public List<Waypoint> Waypoints { get; set; }
    [JsonPropertyName("code")]
    public string Code { get; set; } // Ok -> for logic check
    [JsonPropertyName("routes")]
    public List<Route> Routes { get; set; }

    public class Route
    {
        [JsonPropertyName("weight_name")]
        public string WeightName { get; set; }
        [JsonPropertyName("weight")]
        public double Weight { get; set; }
        [JsonPropertyName("duration")]
        public decimal Duration { get; set; }
        [JsonPropertyName("distance")]
        public decimal Distance { get; set; }
        [JsonPropertyName("geometry")]
        public RouteGeometry Geometry { get; set; }

        public class RouteGeometry
        {
            [JsonPropertyName("type")]
            public string Type { get; set; }

            [JsonPropertyName("coordinates")]
            public List<List<double>> Coordinates { get; set; }
        }
    }

    public class Waypoint
    {
        [JsonPropertyName("distance")]
        public double Distance { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("location")]
        public List<double> Location { get; set; }
    }
}

// Google
public class GoogleRouteResponse
{
    public List<RouteResponseBody> Routes { get; set; } = [];

    public class RouteResponseBody
    {
        public int DistanceMeters { get; set; }
        public string Duration { get; set; }
    }
}

public class RouteErrorResponse
{
    public RouteError Error { get; set; }
    public class RouteError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
