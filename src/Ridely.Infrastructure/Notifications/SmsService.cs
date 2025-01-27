using Soloride.Application.Abstractions.Notifications;

namespace Soloride.Infrastructure.Notifications;
internal sealed class SmsService : ISmsService
{
    private readonly TermiiService _termiiService;

    public SmsService(TermiiService termiiService)
    {
        _termiiService = termiiService;
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        await Task.CompletedTask;
    }

    public async Task<bool> SendVerificationCodeAsync(string phoneNo, string otp, string expiryDurationInMinutes)
    {
        var result = await _termiiService.SendAsync(phoneNo, otp, expiryDurationInMinutes);

        return result;
    }
}
