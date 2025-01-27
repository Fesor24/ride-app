using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.VoiceCall.EndCall
{
    public sealed record EndCallCommand(int CallId) : ICommand;
}
