namespace Ridely.Application.Abstractions.Notifications;
public interface ISmsService
{
    Task SendAsync(string phoneNumber, string message);
    Task<bool> SendVerificationCodeAsync(string phoneNo, string otp, 
        string expiryDurationInMinutes, MessageMedium messageMedium);
}
