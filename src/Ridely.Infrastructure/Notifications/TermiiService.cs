using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ridely.Application.Abstractions.Notifications;

namespace Ridely.Infrastructure.Notifications;
internal sealed class TermiiService
{
    private readonly HttpClient _httpClient;
    private readonly TermiiOptions _termiiSettings;

    public TermiiService(HttpClient httpClient, IOptions<TermiiOptions> termiiSettings)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(termiiSettings.Value.BaseAddress);
        _httpClient.DefaultRequestHeaders.Add("v1", "test");

        _termiiSettings = termiiSettings.Value;
    }

    public async Task<bool> SendAsync(string phoneNo, string otp, 
        string expiryDurationInMinutes, MessageMedium messageMedium)
    {
        if (string.IsNullOrWhiteSpace(phoneNo)) return false;

        if (phoneNo.StartsWith("+234"))
            phoneNo = phoneNo.Replace("+", "");

        if (phoneNo.StartsWith("0"))
            phoneNo = "234" + phoneNo[1..];

        if (messageMedium == MessageMedium.Sms)
            await SendViaSmsAsync(phoneNo, otp, expiryDurationInMinutes);

        else if(messageMedium == MessageMedium.Whatsapp)
            await SendViaWhatsappAsync(phoneNo, otp, expiryDurationInMinutes);

        return true;
    }

    private async Task SendViaWhatsappAsync(string phoneNo, string otp, string expiryInMinutes)
    {
        var request = new
        {
            api_key = _termiiSettings.ApiKey,
            to = phoneNo,
            sms = otp,
            channel = "whatsapp_otp",
            type = "plain",
            from = "N-Alert",
            time_in_minutes = $"{expiryInMinutes} minutes"
        };

        var jsonRequest = JsonConvert.SerializeObject(request);

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("/api/sms/send", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine(JsonConvert.SerializeObject(content));
    }

    private async Task SendViaSmsAsync(string phoneNo, string otp, string expiryInMinutes)
    {
        string message = $"Your (Ridely) Verification Pin is {otp}. Valid for {expiryInMinutes} " +
            $"minutes, one-time use only.(Ridely)";

        var request = new
        {
            api_key = _termiiSettings.ApiKey,
            to = phoneNo,
            sms = message,
            channel = "dnd",
            type = "plain",
            from = "N-Alert"
        };

        var jsonRequest = JsonConvert.SerializeObject(request);

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync("/api/sms/send", content);

        var responseBody = await response.Content.ReadAsStringAsync();

        Console.WriteLine(JsonConvert.SerializeObject(content));
    }
}
