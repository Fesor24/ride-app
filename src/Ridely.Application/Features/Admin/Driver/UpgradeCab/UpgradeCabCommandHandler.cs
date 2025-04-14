using Ridely.Application.Abstractions.Messaging;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;

namespace Ridely.Application.Features.Admin.Driver.UpgradeCab;
internal sealed class UpgradeCabCommandHandler : 
    ICommandHandler<UpgradeCabCommand>
{
    private readonly ICabRepository _cabRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpgradeCabCommandHandler(ICabRepository cabRepository, IUnitOfWork unitOfWork)
    {
        _cabRepository = cabRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> Handle(UpgradeCabCommand request, CancellationToken cancellationToken)
    {
        var cab = await _cabRepository.GetAsync(request.CabId);

        if (cab is null) return Error.NotFound("cab.notfound", "Cab not found");

        cab.UpgradeToPremium();

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
