using MediatR;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Application.Features.Admin.Settings.Command.UpdateRideSettings;
internal class UpdateRideSettingsCommandHandler : 
    IRequestHandler<UpdateRideSettingsCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISettingsRepository _settingsRepository;

    public UpdateRideSettingsCommandHandler(IUnitOfWork unitOfWork, ISettingsRepository settingsRepository)
    {
        _unitOfWork = unitOfWork;
        _settingsRepository = settingsRepository;
    }

    public async Task<Result<bool>> Handle(UpdateRideSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository
            .GetAllAsync();

        var rideSetting = settings.First();

        rideSetting.RatePerKilometer = request.RatePerKilometer;
        rideSetting.RatePerMinute = request.RatePerMinute;
        rideSetting.BaseFare = request.BaseFare;
        rideSetting.PremiumCab = request.PremiumCab;
        rideSetting.DeliveryRatePerKilometer = request.DeliveryRatePerKilometer;
        rideSetting.DriverCommissionFromRide = request.DriverCommission;

        _settingsRepository.Update(rideSetting);

        return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
    }
}
