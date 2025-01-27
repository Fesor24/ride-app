using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.CreateBankAccount;
public record CreateBankAccountCommand(
    long DriverId,
    long BankId,
    string AccountNo,
    string Otp) :
    ICommand;
