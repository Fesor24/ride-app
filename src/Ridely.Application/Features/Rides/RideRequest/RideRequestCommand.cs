using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Rides;

namespace Ridely.Application.Features.Rides.RideRequest;
public sealed record RideRequestCommand(long RideId,
    PaymentMethod PaymentMethod,
    MusicGenre MusicGenre,
    bool? RideConversation,
    long? PaymentCardId,
    RideCategory RideCategory,
    long RiderId) :
    ICommand<RideRequestResponse>;
