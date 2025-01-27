using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.VoiceCall.StartCall
{
    public sealed record StartCallCommand(int RideId, bool DriverCalled) : 
        ICommand<StartCallResponse>;
}
