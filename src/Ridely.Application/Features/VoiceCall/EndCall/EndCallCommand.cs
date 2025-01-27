using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.VoiceCall.EndCall
{
    public sealed record EndCallCommand(int CallId) : ICommand;
}
