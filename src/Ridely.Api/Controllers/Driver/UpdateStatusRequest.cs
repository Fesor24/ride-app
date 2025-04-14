using Ridely.Application.Features.Drivers.UpdateStatus;

namespace RidelyAPI.Controllers.Driver;

public sealed class UpdateStatusRequest
{
    public UpdateDriverStatusEnum Status { get; set; }
}
