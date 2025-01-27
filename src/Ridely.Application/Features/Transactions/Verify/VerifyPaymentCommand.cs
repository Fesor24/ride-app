using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Transactions.Verify;
public sealed record VerifyPaymentCommand(string Reference) : ICommand;
