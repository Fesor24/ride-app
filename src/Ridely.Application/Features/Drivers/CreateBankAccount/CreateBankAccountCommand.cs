using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.CreateBankAccount;
public record CreateBankAccountCommand(
    long DriverId,
    long BankId,
    string AccountNo,
    string Otp) :
    ICommand;
