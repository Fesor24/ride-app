using System.ComponentModel.DataAnnotations.Schema;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Riders;

namespace Ridely.Domain.Rides;
public sealed class Ratings : Entity
{
    private Ratings()
    {
        
    }

    public Ratings(int rating, 
        long rideId, string? feedback)
    {
        if (rating < 1)
            throw new ApplicationException("Rating must be between 1-5");

        if (rating > 5)
            throw new ApplicationException("Rating must be between 1-5");

        if (!string.IsNullOrWhiteSpace(feedback) && feedback.Length > 200)
            throw new ApplicationException("Feedback: Maximum of 200 characters");

        Rating = rating;
        RideId = rideId;
        Feedback = feedback;
    }

    public long RideId { get; set; }
    [ForeignKey(nameof(RideId))]
    public Ride Ride { get; set; }
    public int Rating { get; set; }
    public string? Feedback { get; set; }
}
