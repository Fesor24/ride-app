using Ridely.Application.Features.Drivers.UpdateStatus;

namespace Ridely.Api.Controllers.Driver;

public sealed class UpdateStatusRequest
{
    public UpdateDriverStatusEnum Status { get; set; }
}
