namespace Ridely.Contracts.Models;
public sealed record RideObject(
    long Id,
    string SourceAddress,
    string DestinationAddress,
    string MusicGenre,
    bool HaveConversation,
    long EstimatedFare,
    string PaymentMethod
    );
