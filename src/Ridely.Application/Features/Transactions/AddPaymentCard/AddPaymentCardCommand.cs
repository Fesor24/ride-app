using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Transactions.AddPaymentCard;
public sealed record AddPaymentCardCommand(long RiderId) : 
    ICommand<AddPaymentCardResponse>;
