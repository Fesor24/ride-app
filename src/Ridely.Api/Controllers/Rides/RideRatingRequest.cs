namespace Ridely.Api.Controllers.Rides;

public class RideRatingRequest
{
    public int Ratings { get; set; }
    public string Feedback { get; set; }
    public int RideId { get; set; }
}
