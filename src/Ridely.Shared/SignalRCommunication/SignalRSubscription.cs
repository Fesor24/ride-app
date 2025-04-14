using System.Net.NetworkInformation;

namespace Ridely.Shared.SignalRCommunication;
public static class SignalRSubscription
{
    // notify driver of request
    // notify rider of request sent to driver
    public static string ReceiveRideRequests = nameof(ReceiveRideRequests);

    // updates concerning a ride
    public static string ReceiveRideUpdates = nameof(ReceiveRideUpdates);

    public static string ReceiveChatMessage = nameof(ReceiveChatMessage);
    public static string ReceiveCallNotification = nameof(ReceiveCallNotification);
    public static string ReceiveLocationUpdate = nameof(ReceiveLocationUpdate);
    public static string ReceiveNearbyDrivers = nameof(ReceiveNearbyDrivers);

    public static string ReceivePaymentUpdates = nameof(ReceivePaymentUpdates);

    public static string EmailVerificationUpdate = nameof(EmailVerificationUpdate);
}

public static class ReceiveRideUpdate
{
    public static string Accepted = nameof(Accepted);
    public static string Cancelled = nameof(Cancelled);
    public static string NoMatch = nameof(NoMatch);
    public static string DriverArrived = nameof(DriverArrived);
    public static string Started = nameof(Started);
    public static string Ended = nameof(Ended);
    public static string Reassigned = nameof(Reassigned);
    public static string Rerouted = nameof(Rerouted);
    public static string WaitTimeExtensionStatus = nameof(WaitTimeExtensionStatus);
    public static string RequestWaitTimeExtension = nameof(RequestWaitTimeExtension);
}
