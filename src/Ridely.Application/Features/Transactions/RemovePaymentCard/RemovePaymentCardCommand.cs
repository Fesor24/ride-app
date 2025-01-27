using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Transactions.RemovePaymentCard;
public sealed record RemovePaymentCardCommand(long PaymentCardId, long RiderId) :
    ICommand;
