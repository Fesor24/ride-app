using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Rides;

namespace Soloride.Application.Features.Rides.RideRequest;
public sealed record RideRequestCommand(long RideId,
    PaymentMethod PaymentMethod,
    MusicGenre MusicGenre,
    bool? RideConversation,
    long? PaymentCardId,
    RideCategory RideCategory,
    long RiderId) :
    ICommand<RideRequestResponse>;
