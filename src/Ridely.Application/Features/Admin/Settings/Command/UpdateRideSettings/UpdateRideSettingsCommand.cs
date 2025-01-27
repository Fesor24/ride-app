using MediatR;
using Soloride.Domain.Abstractions;

namespace Soloride.Application.Features.Admin.Settings.Command.UpdateRideSettings;
public record UpdateRideSettingsCommand(
    decimal BaseFare,
    decimal RatePerKilometer,
    decimal DeliveryRatePerKilometer,
    decimal DriverCommission,
    decimal RatePerMinute,
    int PremiumCab
    ) : 
    IRequest<Result<bool>>;
