using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Application.Features.Common.Banks.Command.Create;
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
