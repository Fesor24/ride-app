using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.DeleteBankAccount;
public sealed record DeleteBankAccountCommand(long BankAccountId, long DriverId) : ICommand;
