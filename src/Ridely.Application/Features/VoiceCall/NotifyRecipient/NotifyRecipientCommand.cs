using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.VoiceCall.NotifyRecipient
{
    public sealed record NotifyRecipientCommand(long RideId, long? DriverId, long? RiderId) :
        ICommand;
}
