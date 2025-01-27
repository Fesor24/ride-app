using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Ridely.Domain.Abstractions;
using Ridely.Shared.Helper;

namespace Ridely.Infrastructure.Route;
internal sealed class RouteService
{
    private readonly HttpClient _httpClient;

    private readonly GoogleRouteOptions _mapboxSettings;

    public RouteService(HttpClient httpClient, IOptions<GoogleRouteOptions> mapboxSettings,
        IOptions<GoogleRouteOptions> googleRouteSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(googleRouteSettings.Value.BaseAddress);
        _httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", googleRouteSettings.Value.ApiKey);
        _httpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask",
            "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

        _mapboxSettings = mapboxSettings.Value;
    }

    // Mapbox
    //public async Task<Result<DirectionsResponse>> GetDistanceAndDurationAsync(DirectionsRequest directions)
    //{
    //    var response = await _httpClient.GetAsync(
    //        $"directions/v5/mapbox/driving/{directions.Origin.Long}," +
    //        $"{directions.Origin.Lat};{directions.Destination.Long},{directions.Destination.Lat}" +
    //        $"?alternatives=true&geometries=geojson&language=en&overview=full&steps=true" +
    //        $"&access_token={_mapboxSettings.AccessToken}");

    //    if (response.IsSuccessStatusCode)
    //    {
    //        var content = await response.Content.ReadAsStringAsync();

    //        if (string.IsNullOrWhiteSpace(content)) return Error.BadRequest("ROUTEDISTANCE_UNAVAILABLE",
    //            "Unable to get the route and distance at the moment");

    //        var directionsResponse = JsonSerializer.Deserialize<DirectionsResponse>(content);

    //        if (directionsResponse is null) return Error.BadRequest("ROUTEDISTANCE_UNAVAILABLE",
    //            "Unable to get the route and distance at the moment");

    //        else if (directionsResponse.Code == "NoRoute") return Error.BadRequest("NO_ROUTE",
    //            "No route available for given coordinates");

    //        else if (directionsResponse.Code == "NoSegment") return Error.BadRequest("NO_ROADSEGMENT",
    //            "No road segment could be matched for one or more coordinates");

    //        else
    //            return directionsResponse;
    //    }
    //    else
    //    {
    //        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    //            return Error.BadRequest("INVALID_TOKEN", "Expired or invalid token passed in request");

    //        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
    //            return Error.BadRequest("ACCOUNT_ISSUES", "Your account has one or more issues which needs to be resolved");

    //        else
    //            return Error.BadRequest("PROFILE_NOTFOUND", "Your profile was not found");
    //    }

    //}

    // using googles route api
    public async Task<Result<RouteResponse.RouteResponseBody>> GetDistanceAndDurationAsync(RouteRequest route)
    {
        var body = JsonContent.Create(route);

        var result = await _httpClient.PostAsync("/directions/v2:computeRoutes", body);

        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStreamAsync();

            var routeResponse = JsonSerializer.Deserialize<RouteResponse>(content, SerializerOptions.Read) ?? new();

            if (routeResponse.Routes.Count > 0)
            {
                return routeResponse.Routes.First();
            }
            else
            {
                return Error.BadRequest("route.notfound", "No route returned from map service");
            }
        }
        else
        {
            var errorContent = await result.Content.ReadAsStreamAsync();

            try
            {
                var errorResponse = JsonSerializer.Deserialize<RouteErrorResponse>(errorContent, SerializerOptions.Read);

                if (errorResponse is not null)
                {
                    return Error.BadRequest(errorResponse.Error.Status, errorResponse.Error.Message);
                }

                return Error.BadRequest("error", "An error occurred");
            }
            catch (Exception)
            {
                return Error.BadRequest("deserialization.error", "Error during deserialization");
            }
        }
    }
}
