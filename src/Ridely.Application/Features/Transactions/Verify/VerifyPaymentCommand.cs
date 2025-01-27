using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Transactions.Verify;
public sealed record VerifyPaymentCommand(string Reference) : ICommand;
