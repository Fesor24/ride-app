using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Ridely.Application.Abstractions.Notifications;

namespace Ridely.Infrastructure.Notifications;
internal sealed class PushNotificationService : IPushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(ILogger<PushNotificationService> logger)
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
                    Body = body,
                    ImageUrl = "https://soloride-public.s3.eu-west-2.amazonaws.com/images/soloride-pushnot-icon.png"
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
                    Body = body,
                    ImageUrl = "https://soloride-public.s3.eu-west-2.amazonaws.com/images/soloride-pushnot-icon.png"
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
