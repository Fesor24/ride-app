using Ridely.Application.Abstractions.Notifications;

namespace Ridely.Infrastructure.Notifications;
internal sealed class SmsService(TermiiService termiiService) : ISmsService
{
    private readonly TermiiService _termiiService = termiiService;

    public async Task SendAsync(string phoneNumber, string message)
    {
        await Task.CompletedTask;
    }

    public async Task<bool> SendVerificationCodeAsync(string phoneNo, string otp, 
        string expiryDurationInMinutes, MessageMedium messageMedium)
    {
        return await Task.FromResult(true);
        
        var result = await _termiiService.SendAsync(phoneNo, otp, 
            expiryDurationInMinutes, messageMedium);

        return result;
    }
}
