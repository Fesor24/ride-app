using Microsoft.AspNetCore.Mvc;
using Ridely.Application.Features.Admin.Rides.GetById;
using Ridely.Application.Features.Admin.Rides.GetRideLogs;
using Ridely.Application.Features.Admin.Rides.Search;
using Ridely.Domain.Models;
using RidelyAPI.Controllers.Base;
using RidelyAPI.Extensions;
using RidelyAPI.Shared;

namespace RidelyAPI.Controllers.Admin.Ride;

[Route("api/rides")]
public class RideController : AdminBaseController<RideController>
{
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(ApiResponse<PaginatedList<SearchRidesResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> SearchRides([FromQuery] SearchRidesRequest request)
    {
        var query = new SearchRidesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Status = [],
            DriverPhoneNo = request.DriverPhoneNo,
            RiderPhoneNo = request.RiderPhoneNo,
            From = request.From,
            To = request.To
        };

        if (request.RideStatus.HasValue) query.Status.Add(request.RideStatus.Value);

        var response = await Sender.Send(query);

        return response.Match(value => Ok(new ApiResponse<PaginatedList<SearchRidesResponse>>(value)),
            this.HandleErrorResult);
    }

    [HttpGet("{rideId}")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<GetRideResponse>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetRide(long rideId)
    {
        var query = new GetRideByIdQuery(rideId);

        var response = await Sender.Send(query);

        return response.Match(value => Ok(new ApiResponse<GetRideResponse>(value)),
            this.HandleErrorResult);
    }

    [HttpGet("{rideId}/logs")]
    [ProducesResponseType(200, Type = typeof(ApiResponse<IReadOnlyList<GetRideLogsResponse>>))]
    [ProducesResponseType(400, Type = typeof(ApiResponse))]
    public async Task<IActionResult> GetLogs(long rideId)
    {
        var query = new GetRideLogsQuery(rideId);

        var response = await Sender.Send(query);

        return response.Match(value => Ok(new ApiResponse<IReadOnlyList<GetRideLogsResponse>>(value)),
            this.HandleErrorResult);
    }
}
