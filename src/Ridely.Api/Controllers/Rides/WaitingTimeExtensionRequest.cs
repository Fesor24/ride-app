namespace RidelyAPI.Controllers.Rides;

public sealed record WaitingTimeExtensionRequest(long RideId, 
    bool AcceptExtension, long RideLogId, string? RejectReason);
