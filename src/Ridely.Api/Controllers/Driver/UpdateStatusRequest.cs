using Soloride.Application.Features.Drivers.UpdateStatus;

namespace SolorideAPI.Controllers.Driver;

public sealed class UpdateStatusRequest
{
    public UpdateDriverStatusEnum Status { get; set; }
}
