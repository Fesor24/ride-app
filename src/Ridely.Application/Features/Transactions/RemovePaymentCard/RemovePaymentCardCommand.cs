using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Transactions.RemovePaymentCard;
public sealed record RemovePaymentCardCommand(long PaymentCardId, long RiderId) :
    ICommand;
