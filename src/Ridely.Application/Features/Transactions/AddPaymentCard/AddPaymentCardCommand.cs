using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Transactions.AddPaymentCard;
public sealed record AddPaymentCardCommand(long RiderId) : 
    ICommand<AddPaymentCardResponse>;
