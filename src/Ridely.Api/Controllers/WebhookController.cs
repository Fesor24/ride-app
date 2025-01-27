using MediatR;
using Microsoft.AspNetCore.Mvc;
using Soloride.Application.Features.Common.Calls.Command;
using SolorideAPI.Controllers.Base;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace SolorideAPI.Controllers;

//public class WebhookController(IVoiceService voiceService) : BaseController<WebhookController>
//{
//    [HttpGet("api/twilio/connect")]
//    public async Task<IActionResult> Connect(string applicationSid, string apiVersion, string from, string callSid)
//    {
//        var response = new VoiceResponse();

//        string[] clientIdSplit = from.Split(":");

//        string client = clientIdSplit[1];

//        Console.WriteLine(from);// sample...client:Anonymous
//        Console.WriteLine(client);
//        Console.WriteLine(callSid); // sample from logs seen...CA629125b4c02aef83d8b8cf6d2bbd260d

//        var res = await Sender.Send(new RouteCallCommand(client));

//        if (res.IsFailure)
//        {
//            Console.WriteLine(res.Error.Code);
//            Console.WriteLine(res.Error.Message);
//            response.Say("Please try again! Unable to reach recipient");
//            response.Hangup();
//            return new TwiMLResult(response);
//        }

//        string? receiver = res.Value;

//        if (string.IsNullOrWhiteSpace(receiver))
//        {
//            Console.WriteLine("Receiver is null");
//            response.Say("Please try again! Unable to reach recipient");
//            response.Hangup();
//            return new TwiMLResult(response);
//        }
        
//        // When a token is generated, add an entry, CALL-DRIVER-PHONENO with value of riders phone no
//        // When we get notified from this webhook, based on the from, we see if we have any
//        // CALL-DRIVER-<FROM> OR CALL-RIDER-<FROM>, then we route call to value...
//        // If it does not exist for now, an error reply...

//        // if (from == "rider") client = "driver";
//        // else if (from == "driver") client = "rider";

//        var dial = new Dial();

//        dial.Client(receiver);

//        response.Append(dial);

//        return new TwiMLResult(response);
//    }

//    [HttpGet("api/twilio/fallback")]
//    public IActionResult Fallback()
//    {
//        var response = new VoiceResponse();

//        response.Say("Please try again!!");
//        response.Hangup();

//        return new TwiMLResult(response);
//    }

//    [HttpGet("api/twilio/status")]
//    public IActionResult Status(string? callSid, string from, string? to, 
//        string callStatus, string duration, string callDuration, string timestamp)
//    {
//        Console.WriteLine("Status callback url hit");
        
//        // call sid is the unique identifier for every call...we can use it to track calls
//        Console.WriteLine($"This is the call sid {callSid}");
//        Console.WriteLine($"{from} initiated the call to {to}"); // format is client:0901121122
        
//        // Status options are queued, initiated, ringing, in-progress,
//        // completed, busy, failed, no-answer
//        Console.WriteLine($"Call status is {callStatus}");

//        Console.WriteLine($"Timestamp is {timestamp}");// format is Wed, 18 Dec 2024 15:24:00 +0000

//        // 1 to many with rides...a ride could have many calls

//        // confirm difference between callDuration and duration...
//        // for now, I think we sum both and use as the duration for the call

//        // TODO: Save records to db for call records...
//        // props..rideId, duration, status...

//        return Ok();
//    }

   
//}
