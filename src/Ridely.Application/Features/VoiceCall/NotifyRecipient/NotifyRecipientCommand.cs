using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.VoiceCall.NotifyRecipient
{
    public sealed record NotifyRecipientCommand(long RideId, long? DriverId, long? RiderId) :
        ICommand;
}
