namespace Ridely.Contracts.Models;
public sealed record RideObject(
    long Id,
    string SourceAddress,
    string DestinationAddress,
    List<string> Waypoints,
    string MusicGenre,
    bool HaveConversation,
    long EstimatedFare,
    string PaymentMethod
    );
