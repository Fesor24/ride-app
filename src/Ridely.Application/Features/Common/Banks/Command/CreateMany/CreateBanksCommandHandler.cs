using AutoMapper;
using MediatR;
using Soloride.Domain.Abstractions;
using Soloride.Domain.Common;

namespace Soloride.Application.Features.Common.Banks.Command.CreateMany;
internal sealed class CreateBanksCommandHandler(IBankRepository bankRepository, IUnitOfWork unitOfWork, IMapper mapper) :
    IRequestHandler<CreateBanksCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateBanksCommand request, CancellationToken cancellationToken)
    {
        //var user = await unitOfWork.UserRepository
        //    .GetAsync(request.UserId);

        //if (user is null) return Error.NotFound("USER_NOTFOUND", "User not found");

        //var adminRole = await unitOfWork.RoleRepository
        //    .GetByCodeAsync(nameof(RoleEnum.Admin));

        //if (adminRole is null) return Error.NotFound("ADMINROLE_NOTFOUND", "Admin role not found");

        //if (user.RoleId != adminRole.Id) return Error.Unauthorized("ADMIN_ONLY", "Only admins can add banks");

        var banks = mapper.Map<List<Bank>>(request.Banks);

        await bankRepository.AddRangeAsync(banks);

        return await unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
