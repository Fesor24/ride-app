using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Drivers.VerifyBankAccount;
public sealed record VerifyBankAccountQuery(string AccountNo, long BankId, long DriverId) :
    IQuery<VerifyBankAccountResponse>;
