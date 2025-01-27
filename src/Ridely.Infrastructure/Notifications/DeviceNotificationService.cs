using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Soloride.Application.Abstractions.Notifications;

namespace Soloride.Infrastructure.Notifications;
internal sealed class DeviceNotificationService : IDeviceNotificationService
{
    private readonly ILogger<DeviceNotificationService> _logger;

    public DeviceNotificationService(ILogger<DeviceNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> PushAsync(string deviceTokenId, string title, string body)
    {
        try
        {
            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Token = deviceTokenId
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            return !string.IsNullOrWhiteSpace(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while pushing notification. Message: {message}. Details: {details}",
                ex.Message, ex.StackTrace);
            return false;
        }
    }

    public async Task<bool> PushAsync(string deviceTokenId, string title, string body,
        Dictionary<string, string> data, string type)
    {
        try
        {
            data.Add("type", type);

            var message = new Message()
            {
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Token = deviceTokenId,
                Data = data
            };

            var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            return !string.IsNullOrWhiteSpace(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "An error occurred while pushing notification. Message: {message}. Details: {details}",
                ex.Message, ex.StackTrace);
            return false;
        }
    }
}
