namespace Ridely.Application.Abstractions.Notifications;
public interface IPushNotificationService
{
    Task<bool> PushAsync(string deviceTokenId, string title, string body);
    Task<bool> PushAsync(string deviceTokenId, string title, string body,
        Dictionary<string, string> data, string type);
}
