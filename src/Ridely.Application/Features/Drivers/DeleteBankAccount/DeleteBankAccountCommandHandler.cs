using Soloride.Application.Abstractions.Messaging;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Drivers;

namespace Soloride.Application.Features.Drivers.DeleteBankAccount;
internal sealed class DeleteBankAccountCommandHandler(IUnitOfWork unitOfWork, IBankAccountRepository bankAccountRepository) :
    ICommandHandler<DeleteBankAccountCommand>
{
    public async Task<Result<bool>> Handle(DeleteBankAccountCommand request, CancellationToken cancellationToken)
    {
        var bankAccount = await bankAccountRepository
            .GetAsync(request.BankAccountId);

        if (bankAccount is null)
            return Error.NotFound("bankaccount.notfound", "Bank account not found");

        bankAccountRepository.Delete(bankAccount);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
