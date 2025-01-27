using Soloride.Application.Abstractions.Messaging;

namespace Soloride.Application.Features.Drivers.VerifyBankAccount;
public sealed record VerifyBankAccountQuery(string AccountNo, long BankId, long DriverId) :
    IQuery<VerifyBankAccountResponse>;
