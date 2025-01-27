using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.DeleteBankAccount;
public sealed record DeleteBankAccountCommand(long BankAccountId, long DriverId) : ICommand;
