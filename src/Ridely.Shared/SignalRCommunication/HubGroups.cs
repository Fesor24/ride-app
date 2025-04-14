namespace Ridely.Shared.SignalRCommunication;
public static class HubGroups
{
    public static string Ride(long rideId) => $"RIDE-GROUP-{rideId}";
    // for chats between parties...
    public static string Chat(long rideId) => $"CHAT-GROUP-{rideId}";
}
