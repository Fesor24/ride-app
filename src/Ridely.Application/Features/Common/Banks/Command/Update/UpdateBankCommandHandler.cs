using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Application.Features.Common.Banks.Command.Update;
internal sealed class UpdateBankCommandHandler(IUnitOfWork unitOfWork, IBankRepository bankRepository) :
    IRequestHandler<UpdateBankCommand, Result<bool>>
{
    //todo: relevant??
    public async Task<Result<bool>> Handle(UpdateBankCommand request, CancellationToken cancellationToken)
    {
        Bank? bank = await bankRepository.GetAsync(request.Id);

        if (bank is null) return Error.NotFound("BANK_NOTFOUND", "Bank not found");

        //bank.Name = request.Name;

        bankRepository.Update(bank);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
