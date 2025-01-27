using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.VoiceCall.StartCall
{
    public sealed record StartCallCommand(int RideId, bool DriverCalled) : 
        ICommand<StartCallResponse>;
}
