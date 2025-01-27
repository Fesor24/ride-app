using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;

namespace Soloride.Application.Features.Common.Banks.Command.Create;
internal sealed class CreateBankCommandHandler(IBankRepository bankRepository, IUnitOfWork unitOfWork) :
    IRequestHandler<CreateBankCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateBankCommand request, CancellationToken cancellationToken)
    {
        Bank bank = new(
            request.Name,
            request.Code
            );

        await bankRepository.AddAsync(bank);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
